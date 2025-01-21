using OneLogin.Core;

var builder = WebApplication.CreateBuilder(args);

//加入cookie的授权验证
//var expireTime = int.Parse(builder.Configuration["LoginSettings:ExpiredTime"] ?? "7200");
//var cookieScheme = builder.Configuration["LoginSettings:CookieScheme"] ?? "sso.kx-code.com";
//builder.AddOneLoginAuthentication(cookieScheme, "/auth/login", "/auth/logout", "/denied", expireTime);
builder.AddOneLoginAuthentication();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
