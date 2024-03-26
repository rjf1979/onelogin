namespace OneLogin.WebUI.Login.Models
{
    public class DemoUserSettingModel
    {
        public IList<DemoUserModel> UserList { get; set; }
    }

    public class DemoUserModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
