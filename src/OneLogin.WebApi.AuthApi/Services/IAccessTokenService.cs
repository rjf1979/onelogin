using OneLogin.Core.Models;

namespace OneLogin.WebApi.AuthApi.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAccessTokenService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="expTime"></param>
        /// <returns></returns>
        string Create(RequestUserModel model, DateTime expTime);
    }
}
