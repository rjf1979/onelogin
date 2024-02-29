using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneLogin.Core;
using OneLogin.Domains.Interfaces;
using OneLogin.Domains.Models.Jwt;

namespace OneLogin.Logic.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtAccessTokenService: IJwtAccessTokenService
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public JwtAccessTokenService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expTime"></param>
        /// <returns></returns>
        public string CreateToken(JwtTokenUserModel model, DateTime expTime)
        {
            var authTime = DateTime.Now;
            var expiresAt = expTime;//到期时间
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.Name),
                new Claim("Sub", model.Sub)
            };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            var issuer = _jwtSettings.Issuer;
            var aud = _jwtSettings.Audience;

            // 5. 根据以上，生成token
            var jwtSecurityToken = new JwtSecurityToken(
                issuer,     //Issuer
                aud,   //Audience
                claims,                          //Claims,
                authTime,                    //notBefore
                expiresAt,    //expires
                signingCredentials               //Credentials
            );

            // 6. 将token变为string
            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
