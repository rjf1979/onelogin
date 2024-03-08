using System;
using OneLogin.Core;
using OneLogin.Core.Models;

namespace OneLogin.Logic.Core.Interfaces
{
    public interface IAccessTokenService:IBaseInjectService
    {
        RequestUserModel GetRequestUser(string token);
        string Create(RequestUserModel model, DateTime expTime);
    }
}
