namespace OneLogin.Core
{
    public class AuthorizeSettings
    {
        public string LoginUrl { get; set; }
        public string LogoutUrl { get; set; }
        public string OwnerUrl { get; set; }
        public string AuthorizeApi { get; set; }
        public int AdminSiteId { get; set; }
        public int ExpireHour { get; set; }
    }
}
