using OneLogin.Core;
using OneLogin.WebUI.Login.Models;

var builder = WebApplication.CreateBuilder(args);

//注入登录用户信息
builder.Services.Configure<DemoUserSettingModel>(builder.Configuration.GetSection("LoginUserSettings"));

//加入session
builder.Services.AddSession();

//加入cookie
var expireTime = int.Parse(builder.Configuration["LoginSettings:ExpiredTime"] ?? "7200");
var cookieScheme = builder.Configuration["LoginSettings:CookieScheme"] ?? "sso.kx-code.com";
builder.AddOneLoginAuthentication(cookieScheme, "/auth/login", "/auth/logout", "/denied", expireTime);

//验证码
builder.Services.AddCaptcha(builder.Configuration);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
