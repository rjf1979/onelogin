using Microsoft.AspNetCore.Mvc;
using OneLogin.Core.Models;
using System.Security.Claims;
using System.Web;

namespace OneLogin.WebUI.Login.Controllers
{
    public abstract class BaseController : Controller
    {
        protected ClaimsPrincipal GetClaimsPrincipal(string cookieScheme, string accessToken, RequestUserModel requestUserModel)
        {
            var claims = new List<Claim>
            {
                new(nameof(requestUserModel.Name), requestUserModel.Name),
                new(nameof(requestUserModel.Id), requestUserModel.Id),
                new (nameof(requestUserModel.Role),requestUserModel.Role),
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

        protected string GetRedirectUriWithAddToken(string returnUrl, string accessToken)
        {
            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl.StartsWith("http://") || returnUrl.StartsWith("https://"))
                {
                    if (returnUrl.IndexOf("?", StringComparison.Ordinal) >= 0)
                    {
                        returnUrl += "&token=" + HttpUtility.UrlEncode(accessToken);
                    }
                    else
                    {
                        returnUrl += "?token=" + HttpUtility.UrlEncode(accessToken);
                    }
                }
            }
            else
            {
                returnUrl = "/";
            }

            return returnUrl;
        }
    }
}
