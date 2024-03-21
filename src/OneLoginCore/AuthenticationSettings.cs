namespace OneLogin.Core
{
    public class AuthenticationSettings
    {
        public string AuthApi { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public int ExpiredTime { get; set; }
    }
}
