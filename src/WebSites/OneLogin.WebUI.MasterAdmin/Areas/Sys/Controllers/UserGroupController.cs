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
    public class UserGroupController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<UserGroup> _logger;

        public UserGroupController(ISqlSugarClient sqlSugarScope, ILogger<UserGroup> logger)
        {
            _sqlSugarClient = sqlSugarScope.GetMasterDbConnection();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            var findItem = await _sqlSugarClient.Queryable<UserGroup>().FirstAsync(x => x.ID == id);
            return View(findItem);
        }
        
        public async Task<LayuiTableResult<UserGroup>> List([FromQuery] int page, [FromQuery] int limit, [FromQuery] int userid, [FromQuery] string searchKey="")
        {
            RefAsync<int> count = new RefAsync<int>();
            var query = _sqlSugarClient.Queryable<UserGroup>().With(SqlWith.NoLock);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.Name.Contains(searchKey));
            }
            var items = await query.ToPageListAsync(page, limit, count);
            if (userid > 0)
            {
                var userInGroups = await _sqlSugarClient.Queryable<UserGroupRelation>().With(SqlWith.NoLock)
                    .Where(x => x.UserID == userid).ToListAsync();
                items.ForEach(x =>
                {
                    x.IsBinding = userInGroups.Exists(y => y.GroupID == x.ID);
                });
            }
            return LayuiTableResult<UserGroup>.Result(items, count.Value);
        }
        
        public async Task<ExecuteResult> Save([FromBody] UserGroup model)
        {
            if(model.ID==0)
            {
                await _sqlSugarClient.Insertable(model).ExecuteCommandAsync();
                return ExecuteResult.Ok("保持成功");
            }
            var findItem = await _sqlSugarClient.Queryable<UserGroup>().FirstAsync(x => x.ID == model.ID);
            if (findItem == null) return ExecuteResult.Error("未找到权限组");
            findItem.Remark = model.Remark;
            findItem.Name = model.Name;
            await _sqlSugarClient.Updateable(findItem).ExecuteCommandAsync();
            return ExecuteResult.Ok("保持成功");
        }

    }
}
