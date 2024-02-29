using SqlSugar;

namespace OneLogin.Domains.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("UserGroupRelations")]
    public class UserGroupRelation
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int GroupId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int UserId { get; set; }

        /// <summary>
        /// Desc:
        /// Default:1
        /// Nullable:True
        /// </summary>
        [SugarColumn(ColumnDataType = "tinyint(1)", IsNullable = false, DefaultValue = "0")]
        public bool IsDefault { get; set; }

    }
}
