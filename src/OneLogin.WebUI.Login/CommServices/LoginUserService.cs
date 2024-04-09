using NETCore.Encrypt.Extensions;
using OneLogin.Core;

namespace OneLogin.WebUI.Login.CommServices
{
    public class LoginUserService: ILoginUserService
    {
        private readonly string _loginUser;

        public LoginUserService(IConfiguration configuration)
        {
            _loginUser = configuration["LoginUser"];
        }


        public async Task<ExecuteResult> ValidateAsync(string username, string password)
        {
            var userInfos = _loginUser.Split('|');
            var result = username.Trim().ToLower() == userInfos[0] && string.Equals(password.Trim(), userInfos[1].MD5(), StringComparison.CurrentCultureIgnoreCase);
            return await Task.FromResult(ExecuteResult.Ok(result));
        }
    }
}
