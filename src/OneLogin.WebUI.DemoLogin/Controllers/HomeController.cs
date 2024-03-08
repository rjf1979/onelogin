using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core.Models;

namespace OneLogin.WebUI.DemoLogin.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData["site"] = _configuration["SiteName"];
            ViewData["name"] = GetClaimValue(nameof(RequestUserModel.Name));
            return View();
        }
    }
}
