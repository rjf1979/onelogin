#if !DEBUG
using Microsoft.AspNetCore.DataProtection;
#endif
using NLog;
using NLog.Web;
using Public.Core;
using Public.Core.Extensions;
using SqlSugar;
using Sysbase.AdminSite.Common;
using Sysbase.Core;
using InjectionExtension = Public.Core.Extensions.InjectionExtension;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews().AddJsonOptions(configure =>
    {
        configure.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        configure.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

#if !DEBUG
var dir = $@"{AppContext.BaseDirectory}tempkeys\";
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dir));
#endif

    var expireHour = (builder.Configuration["CookieSettings:ExpireHour"]?.ToInt() ?? 12) + 8;//增加8个时区的小时时间
    builder.Services.AddSession(opt =>
    {
        opt.IdleTimeout = TimeSpan.FromHours(expireHour);
    });
    builder.Services
        .AddAuthentication(UserClaimKeys.CookieScheme)
        .AddCookie(UserClaimKeys.CookieScheme, opt =>
        {
            opt.SlidingExpiration = true;
            opt.ClaimsIssuer = "Eper";
            opt.ExpireTimeSpan = TimeSpan.FromHours(expireHour);
            opt.LoginPath = new PathString("/login");
            opt.LogoutPath = new PathString("/logout");
            opt.AccessDeniedPath = new PathString("/denied");//未授权跳转到页面
        });
    builder.Services.Configure<AuthorizeSettings>(builder.Configuration.GetSection("AuthorizeSettings"));
    builder.Host.UseNLog();
    builder.Services.AddHttpClient();
    //builder.Services.AddMemoryCache();
    //builder.Services.AddHybridCacheAsync(builder.Configuration.GetConnectionString("RedisConnection"));
    builder.Services.AddSqlSugar(builder.Configuration, DbType.MySql, InjectionExtension.SqlSugarInjectType.Scope, DataSourceSettings.MasterDb, DataSourceSettings.LogData, DataSourceSettings.PayDb);

    #region -- inject bns service
    builder.Services.AddScopedService("Sysbase.DomainServices");
    #endregion

    var app = builder.Build();

    app.UseStaticFiles();

    app.UseRouting();
    app.UseSession();
    app.UseCookiePolicy();

    app.UseAuthentication();
    app.UseAuthorization();

    //自定义授权验证
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        endpoints.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
        );
    });
    app.Use(async (context, next) =>
    {
        await next.Invoke();
        if (context.Response.StatusCode != 302 && context.Response.StatusCode != 200)
        {
            context.Response.Redirect($"/error/{context.Response.StatusCode}");
        }
    });
    //
    ISqlSugarClient sqlSugarClient = app.Services.GetService<ISqlSugarClient>();
    BuildTableHelper.Build(sqlSugarClient);
    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}