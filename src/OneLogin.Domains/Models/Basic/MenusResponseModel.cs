using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace OneLogin.Domains.Models.Basic
{
    public class MenusResponseModel
    {
        /// <summary>
        /// 权限菜单树
        /// </summary>
        [JsonPropertyName("menuInfo")]
        public IList<MenuNodeModel> MenuInfo { get; set; }

        /// <summary>
        /// logo
        /// </summary>
        [JsonPropertyName("logoInfo")]
        public LogoInfoModel LogoInfo { get; set; }

        /// <summary>
        /// Home
        /// </summary>
        [JsonPropertyName("homeInfo")]
        public HomeInfoModel HomeInfo { get; set; }
    }
}
