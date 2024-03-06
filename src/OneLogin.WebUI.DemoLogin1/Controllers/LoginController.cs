using System.Web;
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
    public class LoginController : Controller
    {
        private readonly ICaptcha _captcha;
        private readonly ILogger<LoginController> _logger;
        private readonly LoginUserSettingModel _loginUserSetting;
        private readonly IConfiguration _configuration;

        public LoginController(ILogger<LoginController> logger, IOptions<LoginUserSettingModel> options, ICaptcha captcha, IConfiguration configuration)
        {
            _logger = logger;
            _captcha = captcha;
            _configuration = configuration;
            _loginUserSetting = options.Value;
        }

        public IActionResult Index(string returnUrl = "")
        {
            //var userName = GetClaimValue(UserClaimKeys.UserName);
            //var token = GetClaimValue(UserClaimKeys.JwtAccessToken);
            //if (!string.IsNullOrEmpty(userName))
            //{
            //    if (!string.IsNullOrEmpty(returnUrl))
            //    {
            //        if (returnUrl.IndexOf("?", StringComparison.Ordinal) >= 0)
            //        {
            //            returnUrl += "&token=" + HttpUtility.UrlEncode(token);
            //        }
            //        else
            //        {
            //            returnUrl += "?token=" + HttpUtility.UrlEncode(token);
            //        }
            //        return Redirect(returnUrl);
            //    }
            //    return Redirect("/");
            //}
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

        public async Task<IActionResult> Do([FromQuery] string username, [FromQuery] string password, [FromQuery] string captchaId, [FromQuery] string captcha)
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
            RequestTokenModel requestTokenModel = new RequestTokenModel
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
            var url = $"{_configuration[""]}/api/Authorize";
            var response = await url.PostJsonAsync(requestTokenModel);
            var jwtTokenResponse = await response.GetJsonAsync<ResponseTokenModel>();
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
    }
}
