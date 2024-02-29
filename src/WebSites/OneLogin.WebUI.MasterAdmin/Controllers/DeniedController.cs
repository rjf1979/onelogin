using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;

namespace Sysbase.AdminSite.Controllers
{
    [AllowAnonymous]
    public class DeniedController : Controller
    {
        public IActionResult Index()
        {
            return Ok(ExecuteResult.Error("无权操作","403"));
        }
    }
}
