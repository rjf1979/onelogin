namespace OneLogin.Domains.Models.Login
{
    public class LoginResponseModel
    {
        public int UserID { get; set; }
        public bool IsAdmin { get; set; }
        public string UserName { get; set; }
        public string ReturnUrl { get; set; }
        public string Token { get; set; }
    }
}
