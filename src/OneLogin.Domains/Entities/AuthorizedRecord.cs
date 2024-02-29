using SqlSugar;

namespace OneLogin.Logic.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("AuthorizedRecords")]
    public class AuthorizedRecord
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
        public int UserGroupID { get; set; }

        public int AdminSiteID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int MenuID { get; set; }

        /// <summary>
        /// 可允许的策略点
        /// </summary>
        public string AllowPolicies { get; set; }
    }
}
