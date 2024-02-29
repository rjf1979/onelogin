using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Public.Core.Extensions;
using Sysbase.Core;
using Sysbase.Domains.Models.Jwt;
using System.Web;
using Sysbase.Domains.Interfaces;

namespace Sysbase.AdminSite.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthorizeApiCrossService _authorizeCrossService;
        private readonly AuthorizeSettings _authorizeSettings;

        public AuthController(IAuthorizeApiCrossService authorizeCrossService, IOptions<AuthorizeSettings> options)
        {
            _authorizeCrossService = authorizeCrossService;
            _authorizeSettings = options.Value;
        }

        public IActionResult In()
        {
            var url = $"{_authorizeSettings.LoginUrl}?ReturnUrl={HttpUtility.UrlEncode(_authorizeSettings.OwnerUrl)}/auth";
            return Redirect(url);
        }

        public async Task<IActionResult> Index(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var result = await _authorizeCrossService.ValidateJwtAccessTokenAsync(token);
                if (result.IsError) return Forbid();
                //记录登陆
                //读取所在用户组授权信息
                var loginReturnData = result.Data.ToJson().ToObject<JwtTokenUserModel>();
                //生成登录信息
                var claimsPrincipal = BaseController.GetClaimsPrincipal(loginReturnData);
                //登录
                await HttpContext.SignInAsync(UserClaimKeys.CookieScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,//在浏览器持久化，false的时候走session持久化
                    ExpiresUtc = new DateTimeOffset(loginReturnData.TokenResponse.ExpireTime.ToDateTime()),
                    RedirectUri = "/"
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
