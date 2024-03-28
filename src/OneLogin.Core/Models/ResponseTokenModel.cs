using System;
using Newtonsoft.Json;

namespace OneLogin.Core.Models
{
    /// <summary>
    /// Token的响应返回
    /// </summary>
    public class ResponseTokenModel
    {
        /// <summary>
        /// 返回消息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// 时间戳，秒计算
        /// </summary>
        [JsonProperty("exp_time")]
        public string ExpireTime { get; set; }

        /// <summary>
        /// 返回的Token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        public static ResponseTokenModel Fail(string msg)
        {
            return new ResponseTokenModel { Message = msg };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public static ResponseTokenModel Ok(string token, DateTime expireTime)
        {
            return new ResponseTokenModel
            {
                Message = "SUCCESS",
                AccessToken = token,
                ExpireTime = expireTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
