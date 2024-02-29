using System.Text.Json.Serialization;

namespace UnifyLogin.WebSite.AdminSite.Models
{
    public class LoginUserModel
    {
        [JsonPropertyName("Username")]
        public string Username { get; set; }
        [JsonPropertyName("Password")]
        public string Password { get; set; }
        [JsonPropertyName("Captcha")]
        public string Captcha { get; set; }
        [JsonPropertyName("ReturnUrl")]
        public string ReturnUrl { get; set; }
    }
}
