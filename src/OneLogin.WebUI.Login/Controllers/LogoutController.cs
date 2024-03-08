using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core;
using OneLogin.Core.Models;
using System.Web;

namespace OneLogin.WebUI.Login.Controllers
{
    [AllowAnonymous]
    public class LogoutController : BaseController
    {
        public IActionResult Index(string returnUrl = "")
        {
            var token = GetClaimValue(nameof(ResponseTokenModel.AccessToken));
            if (string.IsNullOrEmpty(token))
            {
                if (string.IsNullOrEmpty(returnUrl))
                    return Redirect("/login");
                return Redirect("/login?returnUrl=" + HttpUtility.UrlEncode(returnUrl));
            }

            var name = GetClaimValue(nameof(RequestTokenModel.UserInfo.Name));
            ViewData["name"] = name;
            return View();
        }

        public async Task<IActionResult> Do()
        {
            await HttpContext.SignOutAsync();
            return Ok(ExecuteResult.Ok("退出成功"));
        }

        public IActionResult Go(string returnUrl)
        {
            return Redirect(returnUrl);
        }
    }
}
