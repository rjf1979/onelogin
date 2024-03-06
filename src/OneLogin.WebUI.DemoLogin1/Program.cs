var builder = WebApplication.CreateBuilder(args);

//加入session
var expireHour = 12 + 8;//增加8个时区的小时时间
builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromHours(expireHour);
});

//加入cookie
var cookieScheme = "OneLogin.kx-code.com";
builder.Services.AddAuthentication(cookieScheme)
    .AddCookie(cookieScheme, opt =>
    {
        opt.SlidingExpiration = true;
        opt.ClaimsIssuer = "";
        opt.ExpireTimeSpan = TimeSpan.FromHours(expireHour);
        opt.LoginPath = new PathString("/login");
        opt.LogoutPath = new PathString("/logout");
        opt.AccessDeniedPath = new PathString("/denied");//未授权跳转到页面
    });

//验证码
builder.Services.AddCaptcha(builder.Configuration);

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
app.UseSession();
app.UseCookiePolicy();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
