using System;

namespace OneLogin.Core.Models
{
    /// <summary>
    /// Token请求模型
    /// </summary>
    public class RequestTokenModel
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        //[Required(ErrorMessage = "名称Key不能为空")]
        public RequestUserModel UserInfo { get; set; }

        /// <summary>
        /// 时间戳，范围在当前时间前后一分钟内
        /// </summary>
        //[Required(ErrorMessage = "时间戳不能为空")]
        public long Timestamp { get; set; }

        /// <summary>
        /// 随机字符串，生成的授权口令不重复
        /// </summary>
        //[Required(ErrorMessage = "口令不能为空")]
        public string Nonce { get; set; }

        /// <summary>
        /// MD5签名，配合SecretKey进行加签
        /// </summary>
        //[Required(ErrorMessage = "签名不能为空")]
        public string Sign { get; set; }

        public bool IsEffectiveTimestamp(int diff)
        {
            var sec = Timestamp - DateTime.Now.ToTimestamp();
            return Math.Abs(sec) <= diff;
        }

        public bool ValidateSign(string secret)
        {
            var str = $"{nameof(UserInfo.Id).ToLower()}={UserInfo.Id}&{nameof(UserInfo.Name).ToLower()}={UserInfo.Name}&{nameof(Timestamp).ToLower()}={Timestamp}&{nameof(Nonce)}={Nonce}&secret={secret}";
            return string.Equals(Sign, str.ToMd5(), StringComparison.CurrentCultureIgnoreCase);
        }

        public void BuildSign(string secret)
        {
            var str = $"{nameof(UserInfo.Id).ToLower()}={UserInfo.Id}&{nameof(UserInfo.Name).ToLower()}={UserInfo.Name}&{nameof(Timestamp).ToLower()}={Timestamp}&{nameof(Nonce)}={Nonce}&secret={secret}";
            Sign = str.ToMd5();
        }
    }
}
