using System.Collections.Generic;
using System.Linq;
using OneLogin.Domains.Entities;
using OneLogin.Domains.Interfaces;
using OneLogin.Domains.Models.Basic;
using OneLogin.Logic.Core.Entities;

namespace OneLogin.Logic.Services
{
    public class MenuDomainService: IMenuDomainService
    {
        /// <summary>
        /// 递归处理数据
        /// </summary>
        /// <param name="systemMenus"></param>
        /// <param name="adminSite"></param>
        public MenusResponseModel GetMenuResult(IList<Menu> systemMenus,AdminSite adminSite)
        {
            MenusResponseModel menusResult = new MenusResponseModel
            {
                HomeInfo = new HomeInfoModel
                {
                    href = adminSite.HomePageUrl,
                    title = "首页"
                },
                LogoInfo = new LogoInfoModel
                {
                    href = "",
                    title = adminSite.Name,
                    image = adminSite.LogoUrl
                }
            };

            if (systemMenus is not { Count: > 0 })
            {
                return menusResult;
            }
            
            //内部循环方法
            IList<MenuNodeModel> BindChild(int parentId,IEnumerable<Menu> menus)
            {
                IList<MenuNodeModel> list = new List<MenuNodeModel>();
                var findMenus = menus.Where(p => p.ParentID == parentId).ToList();
                if (findMenus is { Count: > 0 })
                {
                    foreach (var item in findMenus)
                    {
                        MenuNodeModel treeNode = item;
                        treeNode.Child = BindChild(treeNode.ID, systemMenus);
                        list.Add(treeNode);
                    }
                }

                return list;
            }

            //开始组装菜单
            menusResult.MenuInfo = BindChild(0, systemMenus);
            return menusResult;
        }
    }
}
