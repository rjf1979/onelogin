namespace OneLogin.Core
{
    public class LoginSettings
    {
        public string AuthApiUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string LoginWebUrl { get; set; }
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public string DeniedPath { get; set; }
        public string CookieScheme { get; set; }
        public int ExpiredTime { get; set; }
    }
}
