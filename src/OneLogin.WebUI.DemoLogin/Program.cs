var builder = WebApplication.CreateBuilder(args);

//����cookie����Ȩ��֤
var expireHour = 12 + 8;//����8��ʱ����Сʱʱ��
var cookieScheme = builder.Configuration["LoginSettings:CookieScheme"] ?? "sso.kx-code.com";
builder.Services.AddAuthentication(cookieScheme)
    .AddCookie(cookieScheme, opt =>
    {
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan = TimeSpan.FromHours(expireHour);
        opt.LoginPath = new PathString("/auth/login");
        opt.LogoutPath = new PathString("/auth/logout");
        opt.AccessDeniedPath = new PathString("/denied");//δ��Ȩ��ת��ҳ��
    });

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
