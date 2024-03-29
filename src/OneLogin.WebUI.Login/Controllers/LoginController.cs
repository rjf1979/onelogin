using Flurl.Http;
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
        private readonly ILoginUserService _loginUserService;
        private readonly string _secretKey;
        private readonly LoginSettings _loginSettings;

        public LoginController(ILogger<LoginController> logger, IOptions<LoginSettings> loginSettingOptions, ICaptcha captcha, IConfiguration configuration, ILoginUserService loginUserService)
        {
            _logger = logger;
            _captcha = captcha;
            _loginUserService = loginUserService;
            _secretKey = configuration["AuthSecretKey"];
            _loginSettings = loginSettingOptions.Value;
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

        public async Task<IActionResult> Do([FromQuery] string username, [FromQuery] string password, [FromQuery] string captchaId, [FromQuery] string captcha, [FromQuery] string retUrl)
        {
            if (!_captcha.Validate(captchaId, captcha))
            {
                return Ok(ExecuteResult.Error("验证码错误"));
            }

            //登陆
            var validateResult = await _loginUserService.ValidateAsync(username, password);
            if (validateResult.IsError)
            {
                return Ok(ExecuteResult.Error("登录失败"));
            }

            //读取所在用户组授权信息
            var expiredTime = DateTime.Now.AddSeconds(_loginSettings.ExpiredTime);

            //获取token
            var requestTokenModel = new RequestTokenModel
            {
                //这里的用户信息，可以自定义再进行扩展，也可以去数据库里读取用户的数据信息
                UserInfo = new RequestUserModel
                {
                    Id = username,
                    //Role = "user",
                    Name = username
                },
                Nonce = Guid.NewGuid().ToString("N"),
                Sign = "",
                Timestamp = DateTime.Now.ToTimestamp()
            };

            requestTokenModel.BuildSign(_secretKey);
            var url = $"{_loginSettings.AuthApiUrl}/api/Auth/Authorize";
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestTokenModel);
            var response = await url.PostJsonAsync(requestTokenModel);
            var jwtTokenResponse = await response.GetJsonAsync<ResponseTokenModel>();
            if (!string.IsNullOrEmpty(jwtTokenResponse.AccessToken))
            {
                //生成登录信息
                var claimsPrincipal = GetClaimsPrincipal(_loginSettings.CookieScheme, jwtTokenResponse.AccessToken, requestTokenModel.UserInfo);
                //登录
                await HttpContext.SignInAsync(_loginSettings.CookieScheme, claimsPrincipal, new AuthenticationProperties
                {
                    IsPersistent = true,//在浏览器持久化，false的时候走session持久化
                    ExpiresUtc = new DateTimeOffset(expiredTime)
                });
                var redirectUrl = GetRedirectUriWithAddToken(retUrl, jwtTokenResponse.AccessToken);
                return Ok(ExecuteResult<string>.Ok(redirectUrl, "登录成功"));
            }
            return Ok(ExecuteResult.Error("登录失败"));
        }
    }
}
