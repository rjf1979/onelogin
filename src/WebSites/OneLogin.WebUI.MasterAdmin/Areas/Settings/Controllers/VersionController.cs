using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;
using SqlSugar;
using Sysbase.Domains.Entities;

namespace Sysbase.AdminSite.Areas.Settings.Controllers
{
    [Authorize]
    [Area("settings")]
    public class VersionController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;

        public VersionController(ISqlSugarClient sqlSugarClient)
        {
            _sqlSugarClient = sqlSugarClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            var findItem = await _sqlSugarClient.Queryable<DeployVersion>().FirstAsync(x => x.ID == id);
            return View(findItem);
        }

        public async Task<LayuiTableResult<Domains.Entities.DeployVersion>> List([FromQuery] int page, [FromQuery] int limit, [FromQuery] string searchKey = "")
        {
            RefAsync<int> count = new RefAsync<int>();
            var query = _sqlSugarClient.Queryable<DeployVersion>();
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.VersionNumber.Contains(searchKey));
            }
            var items = await query.ToPageListAsync(page, limit, count);
            var projectIDs = items.Select(x => x.ProjectID).Distinct().ToList();
            var projects = await _sqlSugarClient.Queryable<SysProject>().Where(x => projectIDs.Contains(x.ID))
                .ToListAsync();
            items.ForEach(x =>
            {
                x.ProjectName = projects.FirstOrDefault(y => y.ID == x.ProjectID)?.ProjectName;
            });
            return LayuiTableResult<DeployVersion>.Result(items, count.Value);
        }

        public async Task<ExecuteResult> Save([FromBody] DeployVersion model)
        {
            if (model.ID == 0)
            {
                model.CreateTime = DateTime.Now;
                await _sqlSugarClient.Insertable(model).ExecuteCommandAsync();
                return ExecuteResult.Ok("保持成功");
            }
            var findItem = await _sqlSugarClient.Queryable<DeployVersion>().FirstAsync(x => x.ID == model.ID);
            findItem.VersionNumber = model.VersionNumber;
            findItem.VersionMessage = model.VersionMessage;
            await _sqlSugarClient.Updateable(findItem).ExecuteCommandAsync();
            return ExecuteResult.Ok("保持成功");
        }
    }
}
