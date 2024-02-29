using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;
using SqlSugar;

namespace Sysbase.AdminSite.Areas.Settings.Controllers
{
    [Authorize]
    [Area("settings")]
    public class ProjectController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public ProjectController(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            var findItem = await _sqlSugarClient.Queryable<Domains.Entities.SysProject>().FirstAsync(x => x.ID == id);
            return View(findItem);
        }

        public async Task<LayuiTableResult<Domains.Entities.SysProject>> List([FromQuery] int page, [FromQuery] int limit, [FromQuery] string searchKey = "")
        {
            RefAsync<int> count = new RefAsync<int>();
            var query = _sqlSugarClient.Queryable<Domains.Entities.SysProject>().With(SqlWith.NoLock);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.ProjectName.Contains(searchKey));
            }
            var items = await query.ToPageListAsync(page, limit, count);
            return LayuiTableResult<Domains.Entities.SysProject>.Result(items, count.Value);
        }

        public async Task<ExecuteResult> Save([FromBody] Domains.Entities.SysProject model)
        {
            if (model.ID == 0)
            {
                model.CreateTime = DateTime.Now;
                await _sqlSugarClient.Insertable(model).ExecuteCommandAsync();
                return ExecuteResult.Ok("保持成功");
            }
            var findItem = await _sqlSugarClient.Queryable<Domains.Entities.SysProject>().FirstAsync(x => x.ID == model.ID);
            findItem.ProjectName = model.ProjectName;
            findItem.ProjectCode = model.ProjectCode;
            await _sqlSugarClient.Updateable(findItem).ExecuteCommandAsync();
            return ExecuteResult.Ok("保持成功");
        }
    }
}
