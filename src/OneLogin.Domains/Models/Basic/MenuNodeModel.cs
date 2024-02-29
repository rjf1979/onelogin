using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using OneLogin.Domains.Entities;

namespace OneLogin.Domains.Models.Basic
{
    public class MenuNodeModel
    {
        /// <summary>
        /// ID
        /// </summary>
        [JsonPropertyName("id")]
        public int ID { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [JsonPropertyName("pid")]
        public int PID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [JsonPropertyName("href")]
        public string Href { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [JsonPropertyName("target")]
        public string Target { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [JsonPropertyName("sort")]
        public int Sort { get; set; }

        /// <summary>
        /// 是否菜单
        /// </summary>
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        /// <summary>
        /// 子集
        /// </summary>
        [JsonPropertyName("child")]
        public IList<MenuNodeModel> Child { get; set; }

        public static implicit operator MenuNodeModel(Menu menu)
        {
            var href = menu.Href;
            if (!string.IsNullOrEmpty(href) && href.IndexOf("menuId", StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                href += "?menuId="+menu.ID;
            }
            return new MenuNodeModel
            {
                Title = menu.Name,
                PID = menu.ParentID,
                Child = new List<MenuNodeModel>(),
                Href = href,
                ID = menu.ID,
                Icon = menu.Icon,
                Sort = menu.Sort,
                Status = true,
                Target = menu.Target
            };
        }
    }
}
