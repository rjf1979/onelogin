namespace OneLogin.Logic.Core.Models.Login
{
    public class VerifyCodeModel
    {
        /// <summary>
        /// 验证码
        /// </summary>
        public string Captcha { get; set; }

        /// <summary>
        /// 验证码图片(字节数组)
        /// </summary>
        public byte[] Image { get; set; }
    }
}
