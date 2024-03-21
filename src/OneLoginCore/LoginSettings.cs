﻿namespace OneLogin.Core
{
    public class LoginSettings
    {
        public string ReturnUrl { get; set; }
        public string LoginDomain { get; set; }
        public string CookieScheme { get; set; }
        public int ExpiredTime { get; set; }
    }
}