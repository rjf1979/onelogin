using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService = OneLogin.Core.AuthenticationService;

namespace OneLogin.WebUI.DemoAdmin.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly AuthenticationService _authenticationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationService"></param>
        public AuthController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// 如果未登录，去登录中心登录
        /// </summary>
        /// <returns></returns>
        public IActionResult Login()
        {
            return _authenticationService.In();
        }

        public async Task<IActionResult> Index(string token)
        {
            return await _authenticationService.Validate(Request.HttpContext, token);
        }

        public async Task<IActionResult> Out()
        {
            return await _authenticationService.Out(Request.HttpContext);
        }
    }
}