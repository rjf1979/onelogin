using NETCore.Encrypt.Extensions;
using OneLogin.Core;

namespace OneLogin.WebUI.Login.CommServices
{
    public class LoginUserService: ILoginUserService
    {
        public async Task<ExecuteResult> ValidateAsync(string username, string password)
        {
            var result = username.Trim().ToLower() == "admin" && string.Equals(password.Trim(), "admin".MD5(), StringComparison.CurrentCultureIgnoreCase);
            return await Task.FromResult(ExecuteResult.Ok(result));
        }
    }
}
