using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;
using SqlSugar;
using Sysbase.Core;
using Sysbase.Domains;
using Sysbase.Domains.Entities;

namespace Sysbase.AdminSite.Areas.Sys.Controllers
{
    [Authorize]
    [Area("sys")]
    public class PermissionController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(ISqlSugarClient sqlSugarScope, ILogger<PermissionController> logger)
        {
            _sqlSugarClient = sqlSugarScope.GetMasterDbConnection();
            _logger = logger;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        public IActionResult ViewMenu()
        {
            return View();
        }

        public IActionResult ViewOperation([FromQuery] int groupId, [FromQuery] int menuId)
        {
            var existsItem = _sqlSugarClient.Queryable<AuthorizedRecord>().First(x =>
                x.MenuID == menuId && x.UserGroupID == groupId);
            //ViewBag.UserGroupPowerID = existsItem.UserGroupPowerID;
            return View(existsItem);
        }

        public IActionResult ViewUserGroup()
        {
            return View();
        }

        //----------------//


        public async Task<LayuiTableResult<Menu>> MenuList([FromQuery] int groupId, [FromQuery] int adminSiteID)
        {
            var menus = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock).Where(x => x.Status == 10 && x.AdminSiteID == adminSiteID).ToListAsync();
            //读取授权配置
            var userGroupPowers = await _sqlSugarClient.Queryable<AuthorizedRecord>().With(SqlWith.NoLock).Where(x => x.UserGroupID == groupId && x.AdminSiteID == adminSiteID)
                .ToListAsync();
            menus.ForEach(x =>
            {
                x.Authorized = userGroupPowers.Any(y => y.MenuID == x.ID);
            });

            return LayuiTableResult<Menu>.Result(menus.OrderByDescending(x => x.Sort).ToList(), menus.Count);
        }

        public async Task<ExecuteResult> SaveBindUserGroup([FromQuery] int userid, [FromQuery] int groupId, [FromQuery] string action)
        {
            if (action == "bind")
            {
                UserGroupRelation userInGroup = new UserGroupRelation
                {
                    UserID = userid,
                    GroupID = groupId,
                    IsDefault = false
                };
                var found = await _sqlSugarClient.Queryable<UserGroupRelation>()
                    .Where(x => x.UserID == userInGroup.GroupID && x.GroupID == userInGroup.GroupID).AnyAsync();
                if (found) return ExecuteResult.Ok("已绑定过");
                await _sqlSugarClient.Insertable(userInGroup).ExecuteCommandAsync();
                return ExecuteResult.Ok("绑定成功");
            }
            else
            {
                await _sqlSugarClient.Deleteable<UserGroupRelation>()
                    .Where(x => x.GroupID == groupId && x.UserID == userid)
                    .ExecuteCommandAsync();
                return ExecuteResult.Ok("解绑成功");
            }
        }

        public async Task<ExecuteResult> AuthorizeMenu([FromQuery] int groupId, [FromQuery] int adminSiteID, [FromQuery] int authorizeMenuId, [FromQuery] int act)
        {
            var findMenu = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                .FirstAsync(x => x.ID == authorizeMenuId);
            if (findMenu == null) return ExecuteResult.Error("未找到菜单");
            IList<AuthorizedRecord> userGroupPowers = new List<AuthorizedRecord>();
            userGroupPowers.Add(new AuthorizedRecord
            {
                UserGroupID = groupId,
                MenuID = authorizeMenuId,
                AllowPolicies = "read",
                AdminSiteID = adminSiteID
            });
            if (act == 1)
            {
                //授权
                //去找父级
                var parentMenuId = findMenu.ParentID;
                while (parentMenuId > 0)
                {
                    //先添加当前父级
                    userGroupPowers.Add(new AuthorizedRecord
                    {
                        UserGroupID = groupId,
                        MenuID = parentMenuId,
                        AllowPolicies = "read",
                        AdminSiteID = adminSiteID
                    });
                    //找寻下一个父级
                    var findId = parentMenuId;
                    var menu = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                        .FirstAsync(x => x.ID == findId);
                    parentMenuId = menu.ParentID;
                }
                //开始更新到数据表
                foreach (var userGroupPower in userGroupPowers)
                {
                    var existsItem = await _sqlSugarClient.Queryable<AuthorizedRecord>().FirstAsync(x =>
                        x.MenuID == userGroupPower.MenuID && x.UserGroupID == userGroupPower.UserGroupID);
                    if (existsItem == null)
                    {
                        await _sqlSugarClient.Insertable(userGroupPower).ExecuteCommandAsync();
                    }
                }
                return ExecuteResult.Ok("授权完成");
            }

            if (act == 0)
            {
                //解除
                //寻找子菜单，解除授权访问
                var childMenus = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                    .Where(x => x.ParentID == authorizeMenuId).ToListAsync();
                foreach (var systemMenu in childMenus)
                {
                    userGroupPowers.Add(new AuthorizedRecord
                    {
                        UserGroupID = groupId,
                        MenuID = systemMenu.ID,
                        AllowPolicies = "read",
                        AdminSiteID = adminSiteID
                    });
                    await FindChildMenu(userGroupPowers, systemMenu);
                }

                //子方法
                async Task FindChildMenu(IList<AuthorizedRecord> userGroupPowerList, Menu menu)
                {
                    var cMenus = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                        .Where(x => x.ParentID == menu.ID).ToListAsync();
                    foreach (var systemMenu in cMenus)
                    {
                        userGroupPowerList.Add(new AuthorizedRecord
                        {
                            UserGroupID = groupId,
                            MenuID = systemMenu.ID,
                            AllowPolicies = "read",
                            AdminSiteID = adminSiteID
                        });
                        await FindChildMenu(userGroupPowerList, systemMenu);
                    }
                }

                //开始更新到数据表
                foreach (var userGroupPower in userGroupPowers)
                {
                    await _sqlSugarClient.Deleteable<AuthorizedRecord>().Where(x =>
                        x.MenuID == userGroupPower.MenuID && x.UserGroupID == userGroupPower.UserGroupID).ExecuteCommandAsync();

                }
                return ExecuteResult.Ok("解权完成");
            }
            return ExecuteResult.Error("未知操作");
        }

        public async Task<ExecuteResult> AuthorizeOpt([FromQuery] int id, [FromQuery] string opt)
        {
            await _sqlSugarClient.Updateable<AuthorizedRecord>()
                .SetColumns(x => x.AllowPolicies == opt)
                .Where(x => x.ID == id)
                .ExecuteCommandAsync();
            return ExecuteResult.Ok("授权完成");
        }

        public async Task<LayuiTableResult<UserGroup>> UserGroupList([FromQuery] int page, [FromQuery] int limit, [FromQuery] int userid, [FromQuery] string showBind, [FromQuery] string searchKey)
        {
            RefAsync<int> count = new RefAsync<int>();
            IList<int> bindedGroupIDs = await _sqlSugarClient.Queryable<UserGroupRelation>().With(SqlWith.NoLock)
                    .Where(x => x.UserID == userid).Select(x => x.GroupID).ToListAsync();
            var query = _sqlSugarClient.Queryable<UserGroup>().With(SqlWith.NoLock);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.Name.Contains(searchKey));
            }

            if (!string.IsNullOrEmpty(showBind))
            {
                if (showBind == "1")
                    query = query.Where(x => bindedGroupIDs.Contains(x.ID));
                if (showBind == "0")
                    query = query.Where(x => !bindedGroupIDs.Contains(x.ID));
            }
            var items = await query.ToPageListAsync(page, limit, count);
            items.ForEach(x =>
                {
                    x.IsBinding = bindedGroupIDs.Any(y => y == x.ID);
                });
            return LayuiTableResult<UserGroup>.Result(items, count.Value);
        }
    }
}
