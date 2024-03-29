using OneLogin.Core;
using OneLogin.WebUI.Login.CommServices;

var builder = WebApplication.CreateBuilder(args);

//����session
builder.Services.AddSession();

//����cookie
builder.AddOneLoginAuthentication();

//��֤��
builder.Services.AddCaptcha(builder.Configuration);

//ע���û���֤����
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
