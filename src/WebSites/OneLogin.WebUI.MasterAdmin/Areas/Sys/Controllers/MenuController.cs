using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;
using SqlSugar;
using Sysbase.Core;
using Sysbase.Domains.Entities;

namespace Sysbase.AdminSite.Areas.Sys.Controllers
{
    [Authorize]
    [Area("Sys")]
    public class MenuController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<MenuController> _logger;

        public MenuController(ISqlSugarClient sqlSugarScope, ILogger<MenuController> logger)
        {
            _sqlSugarClient = sqlSugarScope.GetMasterDbConnection();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewMenuName()
        {
            return View();
        }

        public async Task<IActionResult> List(int adminSiteID)
        {
            if (adminSiteID > 0)
            {
                var menus = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                    .Where(x => x.AdminSiteID == adminSiteID).ToListAsync();
                return Ok(LayuiTableResult<Menu>.Result(menus.OrderByDescending(x => x.Sort).ToList(), menus.Count));
            }

            return Ok(LayuiTableResult<Menu>.Result(new List<Menu>(), 0));
        }

        public async Task<IActionResult> Edit(int id,int pid)
        {
            if (id > 0)
            {
                var systemMenu = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                    .FirstAsync(x => x.ID == id);
                return View(systemMenu);
            }

            return View(new Menu
            {
                ParentID = pid,
                ID = 0,
                Sort = 1,
                ChildMenus = null,
                Href = "",
                Icon = "",
                Target = "_self",
                Name = ""
            });
        }
        
        public async Task<ExecuteResult> Save([FromBody] Menu menu)
        {
            if (menu.ID > 0)
            {
                var findItem = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                    .FirstAsync(x => x.ID == menu.ID);
                if (findItem == null) return ExecuteResult.Error("未找到菜单");
                findItem.Href = menu.Href;
                findItem.Icon = menu.Icon;
                findItem.Sort = menu.Sort;
                findItem.Target = menu.Target;
                findItem.Name = menu.Name;
                var row = await _sqlSugarClient.Updateable(findItem).ExecuteCommandAsync();
                if (row <= 0) return ExecuteResult.Error("保存失败");
                return ExecuteResult.Ok();
            }
            else
            {
                if (menu.ParentID > 0)
                {
                    var parentMenu = await _sqlSugarClient.Queryable<Menu>().FirstAsync(x => x.ID == menu.ParentID);
                    menu.AdminSiteID = parentMenu.AdminSiteID;
                }
                else
                {
                    if (menu.AdminSiteID == 0) return ExecuteResult.Error("请选择站点");
                }
                menu.Status = 10;//默认开启
                var row = await _sqlSugarClient.Insertable(menu).ExecuteCommandAsync();
                if (row <= 0) return ExecuteResult.Error("保存失败");
                return ExecuteResult.Ok();
            }
        }

        public async Task<ExecuteResult> ChangeParentMenu([FromQuery] int id, [FromQuery] int pid)
        {
            var findItem = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                .FirstAsync(x => x.ID == id);
            if (findItem == null) return ExecuteResult.Error("未找到菜单");
            await _sqlSugarClient.Updateable<Menu>()
                .SetColumns(x => x.ParentID == pid)
                .Where(x => x.ID == id)
                .ExecuteCommandAsync();
            return ExecuteResult.Ok("调整成功");
        }

        public async Task<ExecuteResult> Sort([FromQuery] int id, [FromQuery] int index)
        {
            var findItem = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                .FirstAsync(x => x.ID == id);
            if (findItem == null) return ExecuteResult.Error("未找到菜单");
            await _sqlSugarClient.Updateable<Menu>()
                .SetColumns(x => x.Sort == index)
                .Where(x => x.ID == id)
                .ExecuteCommandAsync();
            return ExecuteResult.Ok();
        }

        public async Task<ExecuteResult> Switch([FromQuery] string type, [FromQuery] int id, [FromQuery] int value)
        {
            if (type == "status")
            {
                var findItem = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                    .AnyAsync(x => x.ID == id);
                if (!findItem) return ExecuteResult.Error("未找到菜单");
                await _sqlSugarClient.Updateable<Menu>()
                    .SetColumns(x => x.Status == value)
                    .Where(x => x.ID == id)
                    .ExecuteCommandAsync();
                return ExecuteResult.Ok();
            }
            return ExecuteResult.Error("未知操作");
        }

        public async IAsyncEnumerable<SelectItem> GetAdminSiteList()
        {
            var list = await _sqlSugarClient.Queryable<Domains.Entities.AdminSite>().With(SqlWith.NoLock).ToListAsync();
            foreach (var adminSite in list)
            {
                yield return new SelectItem
                {
                    Text = adminSite.Name,
                    Value = adminSite.ID.ToString()
                };
            }
        }
    }
}
