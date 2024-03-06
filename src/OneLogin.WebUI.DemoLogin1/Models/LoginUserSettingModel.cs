namespace OneLogin.WebUI.Login.Models
{
    public class LoginUserSettingModel
    {
        public IList<LoginUserModel> UserList { get; set; }
    }

    public class LoginUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
