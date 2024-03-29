using OneLogin.Core;
using OneLogin.WebUI.Login.CommServices;

var builder = WebApplication.CreateBuilder(args);

//加入session
builder.Services.AddSession();

//加入cookie
builder.AddOneLoginAuthentication();

//验证码
builder.Services.AddCaptcha(builder.Configuration);

//注入用户验证服务
builder.Services.AddScoped<ILoginUserService,LoginUserService>();

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
