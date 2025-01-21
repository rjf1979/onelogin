using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core;

namespace OneLogin.WebUI.DemoAdmin.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="authenticationService"></param>
        public AuthController(IAuthenticationService authenticationService)
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

        public async Task<IActionResult> Logout()
        {
            return await _authenticationService.Out(Request.HttpContext);
        }
    }
}