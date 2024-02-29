namespace UnifyLogin.WebSite.AdminSite.Areas.Sys.Models
{
    public class ResetPwdModel
    {
        public int ID { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }
    }
}
