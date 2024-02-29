namespace OneLogin.Core
{
    public class UserClaimKeys
    {
        public static string UserId => nameof(UserId);
        public static string Role => nameof(Role);
        public static string JwtAccessToken => nameof(JwtAccessToken);
        public static string UserName => nameof(UserName);
        public static string UserGroupId => nameof(UserGroupId);
        public static string CookieScheme => "Eper.UnifyLogin.Cookies";
        public static string ExpiredTime => nameof(ExpiredTime);
    }
}
