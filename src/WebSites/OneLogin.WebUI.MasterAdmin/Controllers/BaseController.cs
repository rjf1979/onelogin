using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Sysbase.Core;
using Sysbase.Domains.Models.Jwt;
using Sysbase.Domains.Models.Login;

namespace Sysbase.AdminSite.Controllers
{
    public class BaseController : Controller
    {
        protected string GetClaimValue(string claimType)
        {
            var userNameClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == claimType);
            if (userNameClaim == null) return string.Empty;
            return userNameClaim.Value;
        }

        public static ClaimsPrincipal GetClaimsPrincipal(JwtTokenUserModel jwtTokenUser)
        {
            var claims = new List<Claim>
            {
                new(UserClaimKeys.UserName, jwtTokenUser.Name),
                new(UserClaimKeys.UserID, jwtTokenUser.Sub),
                new(ClaimTypes.Role, jwtTokenUser.Role),
                new(UserClaimKeys.JwtAccessToken, jwtTokenUser.TokenResponse.AccessToken),
                new(UserClaimKeys.ExpiredTime,jwtTokenUser.TokenResponse.ExpireTime)
            };
            var claimsIdentity = new ClaimsIdentity(claims, UserClaimKeys.CookieScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }

        public static ClaimsPrincipal GetClaimsPrincipal(string accessToken, DateTime expiredTime, LoginResponseModel loginReturnData)
        {
            var claims = new List<Claim>
            {
                new(UserClaimKeys.UserName, loginReturnData.UserName),
                new(UserClaimKeys.UserID, loginReturnData.UserID.ToString()),
                //new (ClaimTypes.Role,loginReturnData.IsAdmin?AuthorizeRole.Admin:AuthorizeRole.User),
                new(UserClaimKeys.JwtAccessToken, accessToken),
                new(UserClaimKeys.ExpiredTime,expiredTime.ToString("yyyy-MM-dd 23:59:59"))
            };
            var claimsIdentity = new ClaimsIdentity(claims, UserClaimKeys.CookieScheme);
            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
