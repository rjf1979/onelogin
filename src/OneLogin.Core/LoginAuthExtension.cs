using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OneLogin.Core
{
    public static class LoginAuthExtension
    {
        public static void AddLoginAuth(this WebApplicationBuilder builder)
        {
            //加入cookie的授权验证
            var expireHour = 12 + 8;//增加8个时区的小时时间
            var cookieScheme = builder.Configuration["LoginSettings:CookieScheme"] ?? "sso.kx-code.com";
            builder.Services.AddAuthentication(cookieScheme)
                .AddCookie(cookieScheme, opt =>
                {
                    opt.SlidingExpiration = true;
                    opt.ExpireTimeSpan = TimeSpan.FromHours(expireHour);
                    opt.LoginPath = new PathString("/auth/login");
                    opt.LogoutPath = new PathString("/auth/logout");
                    opt.AccessDeniedPath = new PathString("/denied");//未授权跳转到页面
                });
        }
    }
}
