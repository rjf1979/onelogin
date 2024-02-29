using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OneLogin.Core;
using OneLogin.Domains.Interfaces;
using OneLogin.Domains.Models.Jwt;
using OneLogin.Domains.Models.Login;

namespace OneLogin.Logic.Services
{
    public class AuthorizeApiCrossService : IAuthorizeApiCrossService
    {
        private readonly ILogger<AuthorizeApiCrossService> _logger;
        private readonly AuthorizeSettings _authorizeSettings;

        public AuthorizeApiCrossService( IOptions<AuthorizeSettings> options, ILogger<AuthorizeApiCrossService> logger)
        {
            _authorizeSettings = options.Value;
            _logger = logger;
        }

        public async Task<ExecuteResult> ValidateJwtAccessTokenAsync(string tokenString)
        {
            var url = $"{_authorizeSettings.AuthorizeApi}/api/Jwt/Validate";
            if (!tokenString.StartsWith("Bearer")) tokenString = "Bearer " + tokenString;
            //var header = new Dictionary<string, string> { { "Authorization", tokenString } };
            var result = await url.WithHeader("Authorization", tokenString).GetJsonAsync<ExecuteResult>();
            if (result?.ResultCode=="200") return ExecuteResult.Ok("");
            return ExecuteResult.Error("授权验证不通过");
        }

        public async Task<ExecuteResult<LoginResponseModel>> RequestLoginAsync(string userName, string pwd, string codeNo, string codeValue)
        {
            var url = $"{_authorizeSettings.AuthorizeApi}/api/login";
            var postData = new
            {
                Username = userName,
                Password = pwd,
                CodeNo = codeNo,
                CodeValue = codeValue
            };
            return await HttpClientUtil.PostReturnObjectAsync<ExecuteResult<LoginResponseModel>>(_httpClientFactory, url, postData.ToJson(), logger: _logger);
        }

        public async Task<JwtTokenResponseModel> GetJwtAccessTokenAsync(JwtTokenRequestModel model)
        {
            var url = $"{_authorizeSettings.AuthorizeApi}/api/jwt/Authorize";
            var response = await HttpClientUtil.PostAsync(_httpClientFactory, url, model.ToJson(), logger: _logger);
            if (string.IsNullOrEmpty(response))
            {
                return JwtTokenResponseModel.Error("授权未成功");
            }

            return response.ToObject<JwtTokenResponseModel>();
        }

        public async Task<VerifyCodeModel> GetVerifyCodeModelAsync(string codeNo)
        {
            var url = $"{_authorizeSettings.AuthorizeApi}/api/login/VerifyCode/{codeNo}";
            return await HttpClientUtil.GetObjectAsync<VerifyCodeModel>(_httpClientFactory, url, logger: _logger);
        }
    }
}
