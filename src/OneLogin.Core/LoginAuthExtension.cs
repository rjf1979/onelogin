using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OneLogin.Core
{
    public static class LoginAuthExtension
    {
        public static void AddOneLoginAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.Configure<LoginSettings>(builder.Configuration.GetSection(nameof(LoginSettings)));
            var loginSettings = builder.Services.BuildServiceProvider().GetService<IOptions<LoginSettings>>()?.Value;
            if (loginSettings == null) { throw new ArgumentNullException(nameof(loginSettings)); }
            //
            builder.Services.AddAuthentication(loginSettings.CookieScheme)
                .AddCookie(loginSettings.CookieScheme, opt =>
                {
                    opt.SlidingExpiration = true;
                    opt.ExpireTimeSpan = TimeSpan.FromSeconds(loginSettings.ExpiredTime);
                    opt.LoginPath = new PathString(loginSettings.LoginPath);
                    opt.LogoutPath = new PathString(loginSettings.LogoutPath);
                    if (!string.IsNullOrEmpty(loginSettings.DeniedPath)) opt.AccessDeniedPath = new PathString(loginSettings.DeniedPath);//未授权跳转到页面
                });
            //
            builder.Services.AddScoped<JwtSecurityTokenHandler>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
        }
    }
}
