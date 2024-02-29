using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;

namespace Sysbase.AdminSite.Controllers
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
            return Ok(ExecuteResult.Ok());
        }

        public IActionResult Go(string returnUrl)
        {
            return Redirect(returnUrl);
        }
    }
}
