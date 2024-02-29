using System.Reflection;
using Public.Core.Extensions;
using SqlSugar;
using Sysbase.Core;
using Sysbase.Domains.Entities;

namespace Sysbase.AdminSite.Common
{
    public class BuildTableHelper
    {
        public static void Build(ISqlSugarClient sqlSugarClient)
        {
            Type[] types = Assembly
                .LoadFrom(Path.Combine(AppContext.BaseDirectory, "Sysbase.Domains.dll"))//如果 .dll报错，可以换成 xxx.exe 有些生成的是exe 
                .GetTypes().Where(it => it.GetInterface(nameof(IEntity)) != null)//命名空间过滤，当然你也可以写其他条件过滤
                .ToArray();//断点调试一下是不是需要的Type，不是需要的在进行过滤
            sqlSugarClient.CodeFirst.InitTables(types);//根据types创建表
            //新增管理人员登陆
            var isAdmin = sqlSugarClient.Queryable<LoginUser>().Any(a => a.IsAdmin);
            if (!isAdmin)
            {
                LoginUser loginUser = new LoginUser
                {
                    LoginName = "admin",
                    IsAdmin = true,
                    CreateTime = DateTime.Now,
                    ID = 0,
                    LastLoginTime = DateTime.Now,
                    Password = "123456".ToMd5(),
                    Status = 10
                };
                sqlSugarClient.Insertable(loginUser).ExecuteCommand();
            }
            //判断站点信息
            var isHasSite = sqlSugarClient.Queryable<Domains.Entities.AdminSite>().Any(x => x.ID == 1);
            if (!isHasSite)
            {
                Domains.Entities.AdminSite adminSite = new Domains.Entities.AdminSite
                {
                    CreateTime = DateTime.Now,
                    ID = 1,
                    EnterUrl = "",
                    HomePageUrl = "",
                    Intro = "基础管理后台",
                    LogoUrl = "",
                    Name = "基础后台"
                };
                sqlSugarClient.Insertable(adminSite).ExecuteCommand();
            }
            //增加菜单
            //sqlSugarClient.Deleteable<Menu>().Where(a => a.ID > 0).ExecuteCommand();
            if (!sqlSugarClient.Queryable<Menu>().Any(a => a.AdminSiteID == 1))
            {
                Menu menu1 = new Menu
                {
                    Name = "基础",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "",
                    ParentID = 0,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
                var fPid = sqlSugarClient.Insertable(menu1).ExecuteReturnIdentity();
                Menu menu2 = new Menu
                {
                    Name = "配置信息",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "",
                    ParentID = fPid,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
               var sPid = sqlSugarClient.Insertable(menu2).ExecuteReturnIdentity();
                Menu menu3 = new Menu
                {
                    Name = "菜单管理",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "/sys/menu",
                    ParentID = sPid,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
                sqlSugarClient.Insertable(menu3).ExecuteCommand();
                Menu menu4 = new Menu
                {
                    Name = "后台管理",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "/sys/adminsite",
                    ParentID = sPid,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
                sqlSugarClient.Insertable(menu4).ExecuteCommand();
                Menu menu5 = new Menu
                {
                    Name = "用户管理",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "/sys/loginUser",
                    ParentID = sPid,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
                sqlSugarClient.Insertable(menu5).ExecuteCommand();
                Menu menu6 = new Menu
                {
                    Name = "用户组管理",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "/sys/userGroup",
                    ParentID = sPid,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
                sqlSugarClient.Insertable(menu6).ExecuteCommand();
                Menu menu7 = new Menu
                {
                    Name = "授权管理",
                    AdminSiteID = 1,
                    Authorized = true,
                    Href = "/sys/permission",
                    ParentID = sPid,
                    Sort = 200,
                    Status = 10,
                    Target = "",
                    Icon = ""
                };
                sqlSugarClient.Insertable(menu7).ExecuteCommand();
            }
        }
    }
}
