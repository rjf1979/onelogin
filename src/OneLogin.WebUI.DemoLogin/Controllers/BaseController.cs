using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OneLogin.Core.Models;

namespace OneLogin.WebUI.DemoAdmin.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ClaimsPrincipal GetClaimsPrincipal(string cookieScheme, string accessToken, RequestUserModel requestUserModel)
        {
            var claims = new List<Claim>
            {
                new(nameof(requestUserModel.Name), requestUserModel.Name),
                new(nameof(requestUserModel.Id), requestUserModel.Id),
                //new (nameof(requestUserModel.Role),requestUserModel.Role),
                new(nameof(ResponseTokenModel.AccessToken), accessToken)
            };
            var claimsIdentity = new ClaimsIdentity(claims, cookieScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        protected string GetClaimValue(string claimType)
        {
            var userNameClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == claimType);
            if (userNameClaim == null) return string.Empty;
            return userNameClaim.Value;
        }

        protected string GetCurrentDomain(IConfiguration configuration)
        {
            return configuration["OwnAddress"] ?? "";
        }
    }
}
