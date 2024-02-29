using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SqlSugar;
using Sysbase.Core;
using Sysbase.Domains;
using Sysbase.Domains.Entities;
using Sysbase.Domains.Interfaces;
using UnifyLogin.WebSite.AdminSite.Models;

namespace Sysbase.AdminSite.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IMenuDomainService _systemSettingService;
        private readonly AuthorizeSettings _authorizeSettings;

        public HomeController(ILogger<HomeController> logger, IOptions<AuthorizeSettings> options, ISqlSugarClient sqlSugarScope, IMenuDomainService systemSettingService)
        {
            _logger = logger;
            _authorizeSettings = options.Value;
            _sqlSugarClient = sqlSugarScope.GetMasterDbConnection();
            _systemSettingService = systemSettingService;
        }

        public async Task<IActionResult> Index()
        {
            var userID = GetClaimValue(UserClaimKeys.UserID);
            if (string.IsNullOrEmpty(userID))
            {
                await HttpContext.SignOutAsync();
            }

            ViewData[UserClaimKeys.UserName] = GetClaimValue(UserClaimKeys.UserName);
            ViewData[UserClaimKeys.UserID] = userID;
            return View();

        }

        public async Task<IActionResult> GetSystemInfo()
        {
            var adminSiteID = _authorizeSettings.AdminSiteID;
            Domains.Entities.AdminSite adminSite = await _sqlSugarClient.Queryable<Domains.Entities.AdminSite>().With(SqlWith.NoLock).FirstAsync(x => x.ID == adminSiteID);

            IList<Menu> systemMenus = await _sqlSugarClient.Queryable<Menu>().With(SqlWith.NoLock)
                .Where(x => x.Status == 10 && x.AdminSiteID == adminSiteID)
                .ToListAsync();
            var menuResult = _systemSettingService.GetMenuResult(systemMenus.OrderByDescending(x => x.Sort).ToList(), adminSite);
            return Ok(menuResult);
        }

        [Route("/error/{errorCode}")]
        public IActionResult Error([FromRoute] string errorCode)
        {
            ErrorViewModel errorViewModel = new ErrorViewModel
            {
                ErrCode = errorCode
            };
            if (errorViewModel.ErrCode == "404")
            {
                errorViewModel.Message = "很抱歉，你访问的页面找不到了";
            }
            else if (errorViewModel.ErrCode == "403")
            {
                errorViewModel.Message = "很抱歉，你没有权限访问";
            }
            else
            {
                errorViewModel.Message = "很抱歉，发生错误了，请联系管理员";
            }
            return View(errorViewModel);
        }
    }
}