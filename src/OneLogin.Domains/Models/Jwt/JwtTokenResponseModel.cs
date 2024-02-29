using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OneLogin.Domains.Models.Jwt
{
    /// <summary>
    /// Token的响应返回
    /// </summary>
    public class JwtTokenResponseModel
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("msg")]
        [JsonPropertyName("msg")]
        public string Message { get; set; }

        /// <summary>
        /// 时间戳，秒计算
        /// </summary>
        [JsonProperty("exp_time")]
        [JsonPropertyName("exp_time")]
        public string ExpireTime { get; set; }

        /// <summary>
        /// 返回的Token
        /// </summary>
        [JsonProperty("access_token")]
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static JwtTokenResponseModel Error(string msg)
        {
            return new JwtTokenResponseModel { Message = msg };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public static JwtTokenResponseModel Ok(string token, DateTime expireTime)
        {
            return new JwtTokenResponseModel
            {
                Message = "success",
                AccessToken = token,
                ExpireTime = expireTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
