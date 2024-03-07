﻿using Flurl.Http;
using Lazy.Captcha.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneLogin.Core;
using OneLogin.Core.Models;
using OneLogin.WebUI.Login.Models;

namespace OneLogin.WebUI.Login.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        private readonly ICaptcha _captcha;
        private readonly ILogger<LoginController> _logger;
        private readonly LoginUserSettingModel _loginUserSetting;
        private readonly IConfiguration _configuration;
        private readonly string _cookieScheme;

        public LoginController(ILogger<LoginController> logger, IOptions<LoginUserSettingModel> options, ICaptcha captcha, IConfiguration configuration)
        {
            _logger = logger;
            _captcha = captcha;
            _configuration = configuration;
            _loginUserSetting = options.Value;
            _cookieScheme = _configuration["LoginSettings:CookieScheme"] ?? "sso.kx-code.com";
        }

        public IActionResult Index(string returnUrl = "")
        {
            var token = GetClaimValue(nameof(ResponseTokenModel.AccessToken));
            if (!string.IsNullOrEmpty(token))
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    var redirectUrl = GetRedirectUriWithAddToken(returnUrl, token);
                    return Redirect(redirectUrl);
                }

                return Redirect("/");
            }
            return View();
        }

        public IActionResult GetCaptcha([FromQuery] string rnd)
        {
            try
            {
                var captchaModel = CommServices.CaptchaService.Build(_captcha, rnd);
                return File(captchaModel.Image, "image/jpeg");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "获取验证码异常");
                throw;
            }
        }

        public async Task<IActionResult> Do([FromQuery] string username, [FromQuery] string password, [FromQuery] string captchaId, [FromQuery] string captcha, [FromQuery]string retUrl)
        {
            if (!_captcha.Validate(captchaId, captcha))
            {
                return Ok(ExecuteResult.Error("验证码错误"));
            }

            //登陆
            if (_loginUserSetting.UserList.FirstOrDefault(a => a.Username == username && a.Password == password) == null)
            {
                return Ok(ExecuteResult.Error("登录失败"));
            }

            //读取所在用户组授权信息
            var expiredTime = DateTime.Now.AddHours(20);
            //获取token
            var requestTokenModel = new RequestTokenModel
            {
                UserInfo = new RequestUserModel
                {
                    Id = username,
                    Role = "user",
                    Name = username
                },
                Nonce = Guid.NewGuid().ToString("N"),
                Sign = "",
                Timestamp = DateTime.Now.ToTimestamp()
            };

            requestTokenModel.BuildSign(_configuration["AuthSettings:SecretKey"]);
            var url = $"{_configuration["AuthSettings:AuthApi"]}/api/Auth/Authorize";
            var response = await url.PostJsonAsync(requestTokenModel);
            var jwtTokenResponse = await response.GetJsonAsync<ResponseTokenModel>();
            if (!string.IsNullOrEmpty(jwtTokenResponse.AccessToken))
            {
                //生成登录信息
                var claimsPrincipal = GetClaimsPrincipal(_cookieScheme, jwtTokenResponse.AccessToken, requestTokenModel.UserInfo);
                //登录
                await HttpContext.SignInAsync(_cookieScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,//在浏览器持久化，false的时候走session持久化
                    ExpiresUtc = new DateTimeOffset(expiredTime)
                });
                var redirectUrl = GetRedirectUriWithAddToken(retUrl, jwtTokenResponse.AccessToken);
                return Ok(ExecuteResult<string>.Ok(redirectUrl,"登录成功"));
            }
            return Ok(ExecuteResult.Error("登录失败"));
        }


    }
}
