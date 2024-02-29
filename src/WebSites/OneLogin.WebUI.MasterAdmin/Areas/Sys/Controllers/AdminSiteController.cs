using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;
using SqlSugar;
using Sysbase.Core;

namespace Sysbase.AdminSite.Areas.Sys.Controllers
{
    [Authorize]
    [Area("Sys")]
    public class AdminSiteController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<AdminSiteController> _logger;

        public AdminSiteController(ISqlSugarClient sqlSugarScope, ILogger<AdminSiteController> logger)
        {
            _sqlSugarClient = sqlSugarScope.GetMasterDbConnection();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Show()
        {
            _logger.LogInformation("进入首页展示");
            var list = await _sqlSugarClient.Queryable<Domains.Entities.AdminSite>().With(SqlWith.NoLock).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            var findItem = await _sqlSugarClient.Queryable<Domains.Entities.AdminSite>().FirstAsync(x => x.ID == id);
            return View(findItem);
        }

        public async Task<LayuiTableResult<Domains.Entities.AdminSite>> List([FromQuery] int page, [FromQuery] int limit, [FromQuery] string searchKey = "")
        {
            RefAsync<int> count = new RefAsync<int>();
            var query = _sqlSugarClient.Queryable<Domains.Entities.AdminSite>().With(SqlWith.NoLock);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.Name.Contains(searchKey));
            }
            var items = await query.ToPageListAsync(page, limit, count);
            return LayuiTableResult<Domains.Entities.AdminSite>.Result(items, count.Value);
        }

        public async Task<ExecuteResult> Save([FromBody] Domains.Entities.AdminSite model)
        {
            if (model.ID == 0)
            {
                model.CreateTime = DateTime.Now;
                await _sqlSugarClient.Insertable(model).ExecuteCommandAsync();
                return ExecuteResult.Ok("保持成功");
            }
            var findItem = await _sqlSugarClient.Queryable<Domains.Entities.AdminSite>().FirstAsync(x => x.ID == model.ID);
            findItem.Intro = model.Intro;
            findItem.HomePageUrl = model.HomePageUrl;
            findItem.Name = model.Name;
            findItem.EnterUrl = model.EnterUrl;
            findItem.LogoUrl = model.LogoUrl;
            await _sqlSugarClient.Updateable(findItem).ExecuteCommandAsync();
            return ExecuteResult.Ok("保持成功");
        }

    }
}
