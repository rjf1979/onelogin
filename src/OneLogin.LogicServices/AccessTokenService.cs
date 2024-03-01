using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OneLogin.Logic.Core.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OneLogin.Core.Models;

namespace OneLogin.Logic.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AccessTokenService : IAccessTokenService
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccessTokenService> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jwtSecurityTokenHandler"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public AccessTokenService(JwtSecurityTokenHandler jwtSecurityTokenHandler, IConfiguration configuration, ILogger<AccessTokenService> logger)
        {
            _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expTime"></param>
        /// <returns></returns>
        public string Create(RequestUserModel model, DateTime expTime)
        {
            var authTime = DateTime.Now;
            var expiresAt = expTime;//到期时间
            // 1. 定义需要使用到的Claims
            var claims = new[]
            {
                new Claim("Name", model.Name),
                new Claim("Id", model.Id),
                new Claim("Role", model.Role)
            };

            // 2. 从 appsettings.json 中读取SecretKey
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:SecretKey"]));

            // 3. 选择加密算法
            var algorithm = SecurityAlgorithms.HmacSha256;

            // 4. 生成Credentials
            var signingCredentials = new SigningCredentials(secretKey, algorithm);

            var issuer = "onelogin";//这些自定义
            var aud = "onelogin";//这些自定义

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
            var token = _jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
