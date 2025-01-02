using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core;
using OneLogin.Core.Models;
using OneLogin.WebUI.Login.CommServices;

namespace OneLogin.WebUI.Login.Controllers
{
    /// <summary>
    /// Jwt授权服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAccessTokenService _jwtService;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// jwt服务
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="jwtService"></param>
        public AuthController(IConfiguration configuration, IAccessTokenService jwtService)
        {
            _jwtService = jwtService;
            _configuration = configuration;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Authorize")]
        public IActionResult Authorize([FromBody] RequestTokenModel model)
        {
            //验证时间戳
            if (!model.IsEffectiveTimestamp(60))
            {
                return Ok(ResponseTokenModel.Fail("时间戳不符"));
            }

            //验证签名
            if (!model.ValidateSign(_configuration["AuthSettings:SecretKey"]))
            {
                return Ok(ResponseTokenModel.Fail("签名错误"));
            }

            //定义Token的有效期
            var expTime = DateTime.Now.AddSeconds(int.Parse(_configuration["AuthSettings:ExpiredTime"]));

            //生成Token
            var token = _jwtService.Create(model.UserInfo, expTime);
            var responseTokenModel = ResponseTokenModel.Ok(token, expTime);
            return Ok(responseTokenModel);
        }

        /// <summary>
        /// 验证，需要在header里加入授权token
        /// </summary>
        /// <returns></returns>
        [HttpGet("Validate")]
        [Authorize]
        public IActionResult Validate()
        {
            return Ok(ExecuteResult.Ok("SUCCESS", "验证成功", "200"));
        }
    }
}
