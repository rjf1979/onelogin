﻿using System;
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
    public interface IAuthenticationService
    {
        IActionResult In();
        RequestUserModel GetRequestUser(string token);
        Task<IActionResult> Validate(HttpContext context, string token);
        Task<IActionResult> Out(HttpContext context);
    }

    public class AuthenticationService: IAuthenticationService
    {
        private readonly LoginSettings _loginSettings;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

        public AuthenticationService(IOptions<LoginSettings> loginSettingsOptions, JwtSecurityTokenHandler jwtSecurityTokenHandler)
        {
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _loginSettings = loginSettingsOptions.Value;
        }

        /// <summary>
        /// 验证登录
        /// </summary>
        /// <returns></returns>
        public IActionResult In()
        {
            var url = $"{_loginSettings.LoginWebUrl}/login?returnUrl={_loginSettings.ReturnUrl}/auth";
            return new RedirectResult(url);
        }

        public RequestUserModel GetRequestUser(string token)
        {
            var jwtToken = _jwtSecurityTokenHandler.ReadJwtToken(token);
            var requestUser = new RequestUserModel();
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
                new(nameof(ResponseTokenModel.AccessToken), accessToken)
            };
            var claimsIdentity = new ClaimsIdentity(claims, cookieScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        public async Task<IActionResult> Validate(HttpContext context, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var url = $"{_loginSettings.AuthApiUrl}/api/Auth/Validate";
                try
                {
                    var response = await url.WithOAuthBearerToken(token).GetAsync();
                    var executeResult = await response.GetJsonAsync<ExecuteResult>();
                    if (executeResult.IsError) return new ForbidResult();
                }
                catch (FlurlHttpException flurlHttpException)
                {
                    if (flurlHttpException.StatusCode == 401)
                    {
                        //未授权的token，跳转到登录页面
                        return await Out(context);
                    }
                }

                //获取登录者信息
                var userInfo = GetRequestUser(token);

                //读取所在用户组授权信息
                var expiredTime = DateTime.Now.AddSeconds(_loginSettings.ExpiredTime);

                //生成登录信息
                var claimsPrincipal = GetClaimsPrincipal(_loginSettings.CookieScheme,token, userInfo);
                //记录登录
                await context.SignInAsync(_loginSettings.CookieScheme, claimsPrincipal, new AuthenticationProperties
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
            return In();
        }
    }
}