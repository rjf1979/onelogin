using System.Security.Claims;
using UnifyLogin.Core;
using UnifyLogin.Domains.Entities;
using UnifyLogin.WebSite.AdminSite.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Public.Core.Extensions;
using SqlSugar;

namespace UnifyLogin.WebSite.AdminSite.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public PermissionAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool isAdmin = (context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value ?? "")
                .Contains(AuthorizeRole.Admin, StringComparison.CurrentCultureIgnoreCase);
            if (!isAdmin)
            {
                var menuId = context.HttpContext.Request.Query["menuId"].ToString().ToInt();
                if (menuId == 0)
                {
                    context.Result = new ForbidResult();
                }
                var userId = context.HttpContext.User.Claims.FirstOrDefault(_ => _.Type == UserClaimKeys.UserID)?.Value
                    .ToInt();
                ISqlSugarClient sqlSugarClient = context.HttpContext.RequestServices.GetService<SqlSugarScope>()
                    .GetDefaultConnection();
                IList<int> userGroupIDs = (await sqlSugarClient.Queryable<UserGroupRelation>().With(SqlWith.NoLock)
                    .Where(_ => _.UserID == userId).ToListAsync()).Select(_ => _.GroupID).ToList();
                IList<AuthorizedRecord> userGroupPowers = await sqlSugarClient.Queryable<AuthorizedRecord>()
                    .With(SqlWith.NoLock).Where(_ => userGroupIDs.Contains(_.UserGroupID)).ToListAsync();
                var authorizationService = context.HttpContext.RequestServices
                    .GetRequiredService<IPermissionAuthorizationHandler>();
                var authorizationResult =
                    authorizationService.HandleAuthorize(context.HttpContext.User, menuId, Name, userGroupPowers);
                if (!authorizationResult.Succeeded)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
