using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Public.Cache.Hybrid;
using Public.Core;
using SqlSugar;
using Sysbase.Domains.Entities;

namespace Sysbase.AdminSite.Areas.Settings.Controllers
{
    [Authorize]
    [Area("settings")]
    public class SettingsDataController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IHybridCacheAsync _hybridCacheAsync;

        public SettingsDataController(ISqlSugarClient sqlSugarClient, IHybridCacheAsync hybridCacheAsync)
        {
            _sqlSugarClient = sqlSugarClient;
            _hybridCacheAsync = hybridCacheAsync;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Edit([FromQuery] int id)
        {
            var findItem = await _sqlSugarClient.Queryable<DeploySettingData>().FirstAsync(x => x.ID == id);
            return View(findItem);
        }

        public async Task<LayuiTableResult<DeploySettingData>> List([FromQuery] int projectId, [FromQuery] int environmentId)
        {
            var query = _sqlSugarClient.Queryable<DeploySettingData>();
            if (projectId > 0)
            {
                query = query.Where(x => x.ProjectID == projectId);
            }
            if (environmentId > 0)
            {
                query = query.Where(x => x.EnvironmentID == environmentId);
            }
            var items = await query.ToListAsync();
            //
            var projectIDs = items.Select(x => x.ProjectID).Distinct().ToList();
            var projects = await _sqlSugarClient.Queryable<SysProject>().Where(x => projectIDs.Contains(x.ID))
                .ToListAsync();
            var environmentIDs = items.Select(x => x.EnvironmentID).Distinct().ToList();
            var environments = await _sqlSugarClient.Queryable<DeployEnvironment>()
                .Where(x => environmentIDs.Contains(x.ID)).ToListAsync();
            items.ForEach(x =>
            {
                x.ProjectName = projects.FirstOrDefault(y => y.ID == x.ProjectID)?.ProjectName + " > " + projects.FirstOrDefault(y => y.ID == x.ProjectID)?.ProjectCode;
                x.EnvironmentName = environments.FirstOrDefault(y => y.ID == x.EnvironmentID)?.EnvironmentName + " > " + environments.FirstOrDefault(y => y.ID == x.EnvironmentID)?.EnvironmentCode;
            });
            return LayuiTableResult<DeploySettingData>.Result(items, items.Count);
        }

        public async Task<ExecuteResult> Save([FromBody] DeploySettingData model)
        {
            if (model.ProjectID == 0)
            {
                //赋值项目ID
                DeployEnvironment deployEnvironment = await _sqlSugarClient.Queryable<DeployEnvironment>().FirstAsync(x => x.ID == model.EnvironmentID);
                model.ProjectID = deployEnvironment.ProjectID;
            }

            if (model.ID == 0)
            {
                //add
                model.CreateTime = DateTime.Now;
                await _sqlSugarClient.Insertable(model).ExecuteCommandAsync();
                return ExecuteResult.Ok("保持成功");
            }

            var findItem = await _sqlSugarClient.Queryable<DeploySettingData>().FirstAsync(x => x.ID == model.ID);
            //edit
            findItem.ValueContent = model.ValueContent;
            await _sqlSugarClient.Updateable(findItem).ExecuteCommandAsync();
            if (findItem.IsPublished)
            {
                await RefreshPublish(findItem.ID);
            }
            return ExecuteResult.Ok("保持成功");
        }

        public async Task<ExecuteResult> Copy([FromQuery]int id, [FromQuery]string version)
        {
            var findItem = await _sqlSugarClient.Queryable<DeploySettingData>().FirstAsync(x => x.ID == id);
            if (findItem != null)
            {
                findItem.VersionNumber = version;
                await _sqlSugarClient.Insertable(findItem).ExecuteCommandAsync();
                return ExecuteResult.Ok("拷贝成功");
            }
            return ExecuteResult.Error("拷贝失败");
        }

        public async Task<ExecuteResult> Publish([FromQuery] int id, [FromQuery] bool value)
        {
            var result = await _sqlSugarClient.Updateable<DeploySettingData>()
                .SetColumns(x => x.IsPublished == value)
                .Where(x => x.ID == id)
                .ExecuteCommandHasChangeAsync();
            if (result)
            {
                await RefreshPublish(id);
                return ExecuteResult.Ok();
            }
            return ExecuteResult.Error("操作失败");
        }

        private async Task RefreshPublish(int id)
        {
            DeploySettingData deploySettingData =
                await _sqlSugarClient.Queryable<DeploySettingData>().FirstAsync(x => x.ID == id);
            SysProject project = await _sqlSugarClient.Queryable<SysProject>().FirstAsync(x => x.ID == deploySettingData.ProjectID);
            DeployEnvironment deployEnvironment = await _sqlSugarClient.Queryable<DeployEnvironment>().FirstAsync(x => x.ID == deploySettingData.EnvironmentID);
            var key = $"SettingsData:{project.ProjectCode}:{deployEnvironment.EnvironmentCode}:{deploySettingData.VersionNumber}";
            if (deploySettingData.IsPublished)
                await _hybridCacheAsync.SetAsync(key, deploySettingData, TimeSpan.FromDays(1));
            else
                await _hybridCacheAsync.RemoveAsync(key);
        }
    }
}
