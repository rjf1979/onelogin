using System;
using SqlSugar;

namespace OneLogin.Domains.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("LoginUsers")]
    public class LoginUser
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
        public string LoginName { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string Password { get; set; }


        /// <summary>
        /// Desc:
        /// Default:DateTime.Now
        /// Nullable:True
        /// </summary>
        [SugarColumn(ColumnDataType = "DATETIME(3)")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// Desc:
        /// Default:DateTime.Now
        /// Nullable:True
        /// </summary>
        [SugarColumn(ColumnDataType = "DATETIME(3)")]
        public DateTime LastLoginTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnDataType = "int")]
        public int Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(ColumnDataType = "tinyint(1)", IsNullable = false, DefaultValue = "0")]
        public bool IsAdmin { get; set; }
    }
}
