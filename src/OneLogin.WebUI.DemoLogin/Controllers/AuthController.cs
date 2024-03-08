using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Flurl.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core;
using OneLogin.Core.Models;

namespace OneLogin.WebUI.DemoLogin.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly string _cookieScheme;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _cookieScheme = _configuration["LoginSettings:CookieScheme"] ?? "sso.kx-code.com";
        }

        /// <summary>
        /// 如果未登录，去登录中心登录
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            var url = $"{_configuration["LoginSettings:LoginDomain"]}/login?returnUrl={HttpUtility.UrlEncode(GetCurrentDomain())}/auth";
            return Redirect(url);
        }

        private RequestUserModel GetRequestUser(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            RequestUserModel requestUser = new RequestUserModel();
            requestUser.Role = jwtToken.Claims.FirstOrDefault(a => a.Type == nameof(requestUser.Role))?.Value;
            requestUser.Id = jwtToken.Claims.FirstOrDefault(a => a.Type == nameof(requestUser.Id))?.Value;
            requestUser.Name = jwtToken.Claims.FirstOrDefault(a => a.Type == nameof(requestUser.Name))?.Value;
            return requestUser;
        }

        public async Task<IActionResult> Index(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var url = $"{_configuration["AuthSettings:AuthApi"]}/api/Auth/Validate";
                var response = await url.WithOAuthBearerToken(token).GetAsync();
                var executeResult = await response.GetJsonAsync<ExecuteResult>();
                if (executeResult.IsError) return Forbid();

                //获取登录者信息
                var userInfo = GetRequestUser(token);

                //读取所在用户组授权信息
                var expiredTime = DateTime.Now.AddHours(20);

                //生成登录信息
                var claimsPrincipal = GetClaimsPrincipal(_cookieScheme,token, userInfo);
                //记录登录
                await HttpContext.SignInAsync(_cookieScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,//在浏览器持久化，false的时候走session持久化
                    ExpiresUtc = new DateTimeOffset(expiredTime)
                });
                return Redirect("/");
            }

            return Forbid();
        }

        public async Task<IActionResult> Out()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/auth/in");
        }
    }
}
