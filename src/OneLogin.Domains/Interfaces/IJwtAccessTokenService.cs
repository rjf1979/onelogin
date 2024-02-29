using System;
using OneLogin.Core;
using OneLogin.Domains.Models.Jwt;

namespace OneLogin.Domains.Interfaces
{
    public interface IJwtAccessTokenService:IBaseInjectService
    {
        string CreateToken(JwtTokenUserModel model, DateTime expTime);
    }
}
