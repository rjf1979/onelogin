using System.Collections.Generic;
using SqlSugar;

namespace OneLogin.Domains.Entities
{
    [SugarTable("Menus")]
    public class Menu
    {
        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsPrimaryKey = true,IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// 后台站点ID
        /// </summary>
        public int AdminSiteID { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Sort { get; set; }
        [SugarColumn(ColumnDataType = "int")]
        public int Status { get; set; }

        [SugarColumn(IsIgnore = true)]
        public IList<Menu> ChildMenus { get; set; }

        [SugarColumn(IsIgnore = true)]
        public bool Authorized { get; set; }
    }
}
