﻿using System;
using OneLogin.Core;
using OneLogin.Logic.Core.Models;

namespace OneLogin.Logic.Core.Interfaces
{
    public interface IAccessTokenService:IBaseInjectService
    {
        string Create(RequestUserModel model, DateTime expTime);
    }
}
