using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Public.Core;
using Public.Core.Extensions;
using SqlSugar;
using Sysbase.Core;
using Sysbase.Domains;
using Sysbase.Domains.Entities;
using UnifyLogin.WebSite.AdminSite.Areas.Sys.Models;

namespace Sysbase.AdminSite.Areas.Sys.Controllers
{
    [Authorize]
    [Area("sys")]
    public class LoginUserController : Controller
    {
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly ILogger<LoginUserController> _logger;

        public LoginUserController(ISqlSugarClient sqlSugarScope, ILogger<LoginUserController> logger)
        {
            _sqlSugarClient = sqlSugarScope.GetMasterDbConnection();
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Password()
        {
            return View();
        }

        public async Task<IActionResult> Edit([FromQuery]int userId)
        {
            if (userId > 0)
            {
                var findItem = await _sqlSugarClient.Queryable<LoginUser>().With(SqlWith.NoLock)
                    .FirstAsync(x => x.ID == userId);
                return View(findItem);
            }
            return View(new LoginUser());
        }

        //---------------//

        public async Task<LayuiTableResult<LoginUser>> List([FromQuery] int page, [FromQuery] int limit, [FromQuery] string searchKey = "")
        {
            var query = _sqlSugarClient.Queryable<LoginUser>().With(SqlWith.NoLock);
            if (!string.IsNullOrEmpty(searchKey))
            {
                query = query.Where(x => x.LoginName.Contains(searchKey));
            }

            RefAsync<int> totalRecord = new RefAsync<int>();
            RefAsync<int> totalPage = new RefAsync<int>();
            var items = await query.OrderByDescending(x => x.CreateTime).ToPageListAsync(page, limit, totalRecord, totalPage);
            //
            items.ForEach(x =>
            {
                x.Password = x.Password.Substring(0, 2) + "******" + x.Password.Substring(x.Password.Length - 2);
            });
            return LayuiTableResult<LoginUser>.Result(items, totalRecord.Value);
        }

        public async Task<ExecuteResult> InitPwd([FromQuery] int userid)
        {
            var user = await _sqlSugarClient.Queryable<LoginUser>().With(SqlWith.NoLock).FirstAsync(x => x.ID == userid);
            if (user == null) return ExecuteResult.Error("未找到用户");
            var row = await _sqlSugarClient.Updateable<LoginUser>()
                .SetColumns(x => x.Password == user.LoginName.ToMd5().ToLower())
                .Where(x => x.ID == user.ID)
                .ExecuteCommandAsync();
            if (row <= 0) return ExecuteResult.Error("重置密码失败");
            return ExecuteResult.Ok(message: "重置密码成功");
        }

        public async Task<ExecuteResult> Save([FromBody] LoginUser user)
        {
            if (user.ID > 0)
            {
                var findItem = await _sqlSugarClient.Queryable<LoginUser>().FirstAsync(x => x.ID == user.ID);
                if (findItem == null)return ExecuteResult.Error("未找到登陆用户");
                await _sqlSugarClient.Updateable<LoginUser>()
                    .SetColumns(x => x.LoginName == user.LoginName)
                    .Where(x => x.ID == user.ID)
                    .ExecuteCommandAsync();
                return ExecuteResult.Ok("保存成功");
            }

            user.Status = 10;
            user.CreateTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            user.Password = user.LoginName.ToMd5();
            await _sqlSugarClient.Insertable(user).ExecuteCommandAsync();
            return ExecuteResult.Ok("保存成功");
        }

        public async Task<ExecuteResult> SwitchAdmin([FromQuery] int userId, [FromQuery] bool isAdmin)
        {
            var findItem = await _sqlSugarClient.Queryable<LoginUser>().FirstAsync(x => x.ID == userId);
            if (findItem == null) return ExecuteResult.Error("未找到登陆用户");
            if (isAdmin && !findItem.IsAdmin)
            {
                await _sqlSugarClient.Updateable<LoginUser>()
                    .SetColumns(x => x.IsAdmin == true)
                    .Where(x => x.ID == userId)
                    .ExecuteCommandAsync();
                return ExecuteResult.Ok("切换成功");
            }
            await _sqlSugarClient.Updateable<LoginUser>()
                .SetColumns(x => x.IsAdmin == false)
                .Where(x => x.ID == userId)
                .ExecuteCommandAsync();
            return ExecuteResult.Ok("切换成功");
        }

        public async Task<ExecuteResult> SwitchStatus([FromQuery] int userId, [FromQuery] int status)
        {
            var findItem = await _sqlSugarClient.Queryable<LoginUser>().FirstAsync(x => x.ID == userId);
            if (findItem == null) return ExecuteResult.Error("未找到登陆用户");
            await _sqlSugarClient.Updateable<LoginUser>()
                    .SetColumns(x => x.Status == 10)
                    .Where(x => x.ID == userId)
                    .ExecuteCommandAsync();
                return ExecuteResult.Ok("切换成功");
            
        }

        public async Task<ExecuteResult> ResetPwd([FromBody] ResetPwdModel model)
        {
            var user = await _sqlSugarClient.Queryable<LoginUser>().With(SqlWith.NoLock).FirstAsync(x => x.ID == model.ID);
            if (user == null) return ExecuteResult.Error("用户不存在");
            if (user.Password.ToLower() != model.OldPwd.ToMd5().ToLower())
            {
                return ExecuteResult.Error("原密码不符");
            }
            var row = await _sqlSugarClient.Updateable<LoginUser>()
                .SetColumns(x => x.Password == model.NewPwd.ToMd5().ToLower())
                .Where(x => x.ID == user.ID)
                .ExecuteCommandAsync();
            if (row <= 0) return ExecuteResult.Error("修改密码失败");
            return ExecuteResult.Ok(message: "修改密码成功");
        }

    }
}
