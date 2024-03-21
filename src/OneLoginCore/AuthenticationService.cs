using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneLogin.Core.Models;

namespace OneLogin.Core
{
    public class AuthenticationService
    {
        private LoginSettings _loginSettings;

        public AuthenticationService(IOptions<LoginSettings> loginSettingsOptions)
        {
            _loginSettings = loginSettingsOptions.Value;
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <returns></returns>
        public IActionResult In()
        {
            var url = $"{_loginSettings.LoginDomain}/login?returnUrl={_loginSettings.ReturnUrl}/auth";
            return new RedirectResult(url);
        }

        private RequestUserModel GetRequestUser(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var requestUser = new RequestUserModel();
            requestUser.Role = jwtToken.Claims.FirstOrDefault(a => a.Type == nameof(requestUser.Role))?.Value;
            requestUser.Id = jwtToken.Claims.FirstOrDefault(a => a.Type == nameof(requestUser.Id))?.Value;
            requestUser.Name = jwtToken.Claims.FirstOrDefault(a => a.Type == nameof(requestUser.Name))?.Value;
            return requestUser;
        }

        private ClaimsPrincipal GetClaimsPrincipal(string cookieScheme, string accessToken, RequestUserModel requestUserModel)
        {
            var claims = new List<Claim>
            {
                new(nameof(requestUserModel.Name), requestUserModel.Name),
                new(nameof(requestUserModel.Id), requestUserModel.Id),
                new (nameof(requestUserModel.Role),requestUserModel.Role),
                new(nameof(ResponseTokenModel.AccessToken), accessToken)
            };
            var claimsIdentity = new ClaimsIdentity(claims, cookieScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        public async Task<IActionResult> Index(HttpContext context, string cookieScheme,string authenticationApi, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var url = $"{authenticationApi}/api/Auth/Validate";
                var response = await url.WithOAuthBearerToken(token).GetAsync();
                var executeResult = await response.GetJsonAsync<ExecuteResult>();
                if (executeResult.IsError) return new ForbidResult();

                //获取登录者信息
                var userInfo = GetRequestUser(token);

                //读取所在用户组授权信息
                var expiredTime = DateTime.Now.AddHours(20);

                //生成登录信息
                var claimsPrincipal = GetClaimsPrincipal(cookieScheme,token, userInfo);
                //记录登录
                await context.SignInAsync(cookieScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,//在浏览器持久化，false的时候走session持久化
                    ExpiresUtc = new DateTimeOffset(expiredTime)
                });
                return new RedirectResult("/");
            }

            return new ForbidResult();
        }

        public async Task<IActionResult> Out(HttpContext context)
        {
            await context.SignOutAsync();
            return new RedirectResult("/auth/in");
        }
    }
}