namespace OneLogin.Domains.Models.Jwt
{
    /// <summary>
    /// Jwt的Token模型
    /// </summary>
    public class JwtTokenUserModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Sub { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public JwtTokenResponseModel TokenResponse { get; set; }
    }
}
