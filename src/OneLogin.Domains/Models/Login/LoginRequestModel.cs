using System.ComponentModel.DataAnnotations;

namespace OneLogin.Domains.Models.Login
{
    /// <summary>
    /// 登陆请求模型
    /// </summary>
    public class LoginRequestModel
    {
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "请填写用户名")]
        public string Username { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "请填写密码")]
        public string Password { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "请填写验证码标识")]
        public string CodeNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "请填写验证码内容")]
        public string CodeValue { get; set; }
    }
}
