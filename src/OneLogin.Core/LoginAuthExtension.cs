using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OneLogin.Core
{
    public static class LoginAuthExtension
    {
        public static void AddOneLoginAuthentication(this WebApplicationBuilder builder, string cookieScheme,string loginPath,string logoutPath,string deniedPath, int expireSecond)
        {
            builder.Services.Configure<LoginSettings>(builder.Configuration.GetSection(nameof(LoginSettings)));
            //
            builder.Services.AddAuthentication(cookieScheme)
                .AddCookie(cookieScheme, opt =>
                {
                    opt.SlidingExpiration = true;
                    opt.ExpireTimeSpan = TimeSpan.FromSeconds(expireSecond);
                    opt.LoginPath = new PathString(loginPath);
                    opt.LogoutPath = new PathString(logoutPath);
                    if (!string.IsNullOrEmpty(deniedPath)) opt.AccessDeniedPath = new PathString(deniedPath);//未授权跳转到页面
                });
            //
            builder.Services.AddScoped<AuthenticationService>();
        }
    }
}
