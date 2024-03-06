using Lazy.Captcha.Core;
using OneLogin.WebUI.Login.Models;

namespace OneLogin.WebUI.Login.CommServices
{
    /// <summary>
    /// 验证码生成类。 
    /// </summary>
    public class CaptchaService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="captcha"></param>
        /// <param name="codeId"></param>
        /// <returns></returns>
        public static CaptchaModel Build(ICaptcha captcha,string codeId)
        {
            var info = captcha.Generate(codeId);
            return new CaptchaModel
            {
                Captcha = info.Code,
                Image = info.Bytes
            };
        }
    }
}
