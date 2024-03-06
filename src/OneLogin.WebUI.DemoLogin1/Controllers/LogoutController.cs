using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OneLogin.WebUI.Login.Controllers
{
    [AllowAnonymous]
    public class LogoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Do()
        {
            await HttpContext.SignOutAsync();
            return Ok("SUCCESS");
        }

        public IActionResult Go(string returnUrl)
        {
            return Redirect(returnUrl);
        }
    }
}
