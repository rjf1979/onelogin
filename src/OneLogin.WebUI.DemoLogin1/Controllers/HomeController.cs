using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core.Models;

namespace OneLogin.WebUI.Login.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index(string returnUrl)
        {
            var token = GetClaimValue(nameof(ResponseTokenModel.AccessToken));
            if (string.IsNullOrEmpty(token))
            {
                return Redirect("/login?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
            }
            return View();
        }
    }
}
