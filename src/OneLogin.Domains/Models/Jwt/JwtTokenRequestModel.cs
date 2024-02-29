using System;
using System.ComponentModel.DataAnnotations;
using OneLogin.Core;

namespace OneLogin.Logic.Core.Models.Jwt
{
    /// <summary>
    /// Token请求模型
    /// </summary>
    public class JwtTokenRequestModel
    {
        /// <summary>
        /// 名称Key
        /// </summary>
        [Required(ErrorMessage = "名称Key不能为空")]
        public string NameKey { get; set; }

        /// <summary>
        /// 时间戳，范围在当前时间前后一分钟内
        /// </summary>
        [Required(ErrorMessage = "时间戳不能为空")]
        public long Timestamp { get; set; }

        [Required(ErrorMessage = "口令不能为空")]
        public string Token { get; set; }

        /// <summary>
        /// 签名，配合Key进行加签
        /// </summary>
        [Required(ErrorMessage = "签名不能为空")]
        public string Sign { get; set; }

        public bool IsEffectiveTimestamp()
        {
            var sec = Timestamp - DateTime.Now.ToTimestamp();
            return sec > 0;
        }

        public bool ValidateSign(string secretKey)
        {
            var str = $"{nameof(NameKey).ToLower()}={NameKey}&{nameof(Timestamp).ToLower()}={Timestamp}&{nameof(Token)}={Token}&pwd={secretKey}";
            return string.Equals(Sign, str.ToMd5(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void BuildSign(string secretKey)
        {
            var str = $"{nameof(NameKey).ToLower()}={NameKey}&{nameof(Timestamp).ToLower()}={Timestamp}&{nameof(Token)}={Token}&pwd={secretKey}";
            Sign = str.ToMd5();
        }
    }
}
