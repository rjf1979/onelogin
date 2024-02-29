using SqlSugar;

namespace OneLogin.Domains.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("UserGroups")]
    public class UserGroup
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Name { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Remark { get; set; }

        /// <summary>
        /// 是否绑定，用在表格显示上
        /// </summary>
        [SugarColumn(IsIgnore = true)]
        public bool IsBinding { get;set; }
    }
}
