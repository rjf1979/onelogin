using System.Threading.Tasks;
using OneLogin.Core;
using OneLogin.Domains.Models.Jwt;
using OneLogin.Domains.Models.Login;
using OneLogin.Logic.Core.Models.Jwt;
using OneLogin.Logic.Core.Models.Login;

namespace OneLogin.Logic.Core.Interfaces
{
    public interface IAuthorizeApiCrossService: IBaseInjectService
    {
        Task<ExecuteResult> ValidateJwtAccessTokenAsync(string tokenString);
        Task<ExecuteResult<LoginResponseModel>> RequestLoginAsync(string userName, string pwd, string codeNo, string codeValue);
        Task<JwtTokenResponseModel> GetJwtAccessTokenAsync(JwtTokenRequestModel model);
        Task<VerifyCodeModel> GetVerifyCodeModelAsync(string codeNo);
    }
}
