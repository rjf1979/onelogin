using System.Collections.Generic;
using OneLogin.Core;
using OneLogin.Domains.Entities;
using OneLogin.Domains.Models.Basic;
using OneLogin.Logic.Core.Entities;

namespace OneLogin.Domains.Interfaces
{
    public interface IMenuDomainService:IBaseInjectService
    {
        MenusResponseModel GetMenuResult(IList<Menu> systemMenus, AdminSite adminSite);
    }
}
