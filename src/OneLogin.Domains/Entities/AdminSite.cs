using System;
using SqlSugar;

namespace OneLogin.Logic.Core.Entities
{
    [SugarTable("AdminSites")]
    public class AdminSite
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Intro { get; set; }
        public string LogoUrl { get; set; }
        public string EnterUrl { get; set; }
        public string HomePageUrl { get; set; }
        [SugarColumn(ColumnDataType = "DATETIME(3)")]
        public DateTime CreateTime { get; set; }
    }
}
