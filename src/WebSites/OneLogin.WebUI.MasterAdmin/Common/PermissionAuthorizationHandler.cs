using System.Security.Claims;
using UnifyLogin.Domains.Entities;
using Microsoft.AspNetCore.Authorization;

namespace UnifyLogin.WebSite.AdminSite.Common
{
    public class PermissionAuthorizationHandler : IPermissionAuthorizationHandler
    {
        public AuthorizationResult HandleAuthorize(ClaimsPrincipal user, int menuId, string policy,IList<AuthorizedRecord> userGroupPowers)
        {
            if (user.IsInRole("Admin"))
            {
                return AuthorizationResult.Success();
            }
            if (userGroupPowers is { Count: > 0 })
            {
                var findPower = userGroupPowers.FirstOrDefault(_ => _.MenuID == menuId);
                    if (findPower != null 
                        && !string.IsNullOrEmpty(findPower.AllowPolicies) 
                        && findPower.AllowPolicies.Contains(policy,StringComparison.CurrentCultureIgnoreCase))
                        return AuthorizationResult.Success();
            }
            return AuthorizationResult.Failed();
        }
    }

    public interface IPermissionAuthorizationHandler
    {
        AuthorizationResult HandleAuthorize(ClaimsPrincipal user, int menuId, string policy, IList<AuthorizedRecord> userGroupPowers);
    }
}
