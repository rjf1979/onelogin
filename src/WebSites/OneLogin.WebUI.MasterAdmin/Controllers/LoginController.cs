using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Public.Core;
using Public.Core.Extensions;
using Sysbase.Core;
using Sysbase.Domains.Models.Jwt;
using Sysbase.Domains.Models.Login;
using System.Web;
using Sysbase.Domains.Interfaces;
using UnifyLogin.WebSite.AdminSite.Models;

namespace Sysbase.AdminSite.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly IAuthorizeApiCrossService _authorizeCrossService;
        private readonly AuthorizeSettings _authorizeSettings;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IOptions<AuthorizeSettings> options, IAuthorizeApiCrossService authorizeCrossService, ILogger<LoginController> logger)
        {
            _authorizeSettings = options.Value;
            _authorizeCrossService = authorizeCrossService;
            _logger = logger;
        }

        public IActionResult Index(string returnUrl)
        {
            var userName = GetClaimValue(UserClaimKeys.UserName);
            var token = GetClaimValue(UserClaimKeys.JwtAccessToken);
            if (!string.IsNullOrEmpty(userName))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    if (returnUrl.IndexOf("?", StringComparison.Ordinal) >= 0)
                    {
                        returnUrl += "&token=" + HttpUtility.UrlEncode(token);
                    }
                    else
                    {
                        returnUrl += "?token=" + HttpUtility.UrlEncode(token);
                    }
                    return Redirect(returnUrl);
                }
                return Redirect("/");
            }
            return View();
        }

        public async Task<IActionResult> GetCaptcha()
        {
            try
            {
                var model = new LoginCodeModel
                {
                    CodeNo = DateTime.Now.ToTimestamp().ToString()
                };
                HttpContext.Session.SetString("LoginCode", model.ToJson());
                var codeModel = await _authorizeCrossService.GetVerifyCodeModelAsync(model.CodeNo);
                return File(codeModel.Image, "image/jpeg");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,"获取验证码异常");
                throw;
            }
        }

        public async Task<ExecuteResult<LoginResponseModel>> Do([FromBody] LoginUserModel model)
        {
            _logger.LogInformation($"用户：{model.Username}，请求登陆");
            var expireHour = _authorizeSettings.ExpireHour;
            var codeModel = HttpContext.Session.GetString("LoginCode")?.ToObject<LoginCodeModel>();
            if (codeModel == null) return ExecuteResult<LoginResponseModel>.Error("验证超时");
            //登陆
            var pwd = model.Password.ToMd5();
            var result = await _authorizeCrossService.RequestLoginAsync(model.Username, pwd, codeModel.CodeNo, model.Captcha);
            if (!result.IsError)
            {
                //读取所在用户组授权信息
                var loginReturnData = result.Data;
                var expiredTime = DateTime.Now.AddHours(expireHour);
                //获取token
                var jwtTokenRequestModel = new JwtTokenRequestModel
                {
                    NameKey = model.Username,
                    TypeKey = "User",
                    Timestamp = expiredTime.ToTimestamp(),
                    Token = loginReturnData.Token
                };
                jwtTokenRequestModel.BuildSign(pwd);
                var jwtTokenResponse = await _authorizeCrossService.GetJwtAccessTokenAsync(jwtTokenRequestModel);
                if (!string.IsNullOrEmpty(jwtTokenResponse.AccessToken))
                {
                    loginReturnData.ReturnUrl = HttpUtility.UrlDecode(model.ReturnUrl);
                    if (!string.IsNullOrEmpty(loginReturnData.ReturnUrl))
                    {
                        if (loginReturnData.ReturnUrl.StartsWith("http://") || loginReturnData.ReturnUrl.StartsWith("https://"))
                        {
                            if (loginReturnData.ReturnUrl.IndexOf("?", StringComparison.Ordinal) >= 0)
                            {
                                loginReturnData.ReturnUrl += "&token=" + HttpUtility.UrlEncode(jwtTokenResponse.AccessToken);
                            }
                            else
                            {
                                loginReturnData.ReturnUrl += "?token=" + HttpUtility.UrlEncode(jwtTokenResponse.AccessToken);
                            }
                        }
                    }
                    else
                    {
                        loginReturnData.ReturnUrl = "/";
                    }
                    //生成登录信息
                    var claimsPrincipal = GetClaimsPrincipal(jwtTokenResponse.AccessToken, expiredTime, loginReturnData);
                    //登录
                    await HttpContext.SignInAsync(UserClaimKeys.CookieScheme, claimsPrincipal, new AuthenticationProperties
                    {
                        IsPersistent = true,//在浏览器持久化，false的时候走session持久化
                        ExpiresUtc = new DateTimeOffset(expiredTime),
                        RedirectUri = loginReturnData.ReturnUrl
                    });
                    _logger.LogInformation($"用户：{model.Username} 登陆成功");
                    return ExecuteResult<LoginResponseModel>.Ok(loginReturnData);
                }
            }
            return ExecuteResult<LoginResponseModel>.Error("登陆失败");
        }
    }
}
