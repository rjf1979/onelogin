using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;
using OneLogin.Core;
using OneLogin.Domains;
using OneLogin.Domains.Entities;
using OneLogin.Domains.Interfaces;
using OneLogin.Domains.Models.Jwt;
using OneLogin.Logic.Core.Models.Jwt;
using SqlSugar;

namespace OneLogin.WebApi.AuthApi.Controllers
{
    /// <summary>
    /// Jwt授权服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly ILogger<JwtController> _logger;
        private readonly ISqlSugarClient _sqlSugarClient;
        private readonly IJwtAccessTokenService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// jwt服务
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="memoryCache"></param>
        /// <param name="configuration"></param>
        /// <param name="sqlSugarClient"></param>
        /// <param name="jwtService"></param>
        public JwtController(ILogger<JwtController> logger, IMemoryCache memoryCache, IConfiguration configuration, ISqlSugarClient sqlSugarClient, IJwtAccessTokenService jwtService)
        {
            _logger = logger;
            _sqlSugarClient = sqlSugarClient;
            _jwtService = jwtService;
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Authorize")]
        public async Task<JwtTokenResponseModel> AuthorizeAsync([FromBody] JwtTokenRequestModel model)
        {
            //记录请求授权
            _logger.LogInformation($"请求授权记录：{model.ToJson()}");

            //如果是生产环境，启用签名
            if (_configuration["IsProduceEnv"].ToBool())
            {
                //验证时间戳
                if (!model.IsEffectiveTimestamp())
                {
                    return JwtTokenResponseModel.Error("时间戳不符");
                }
            }

            //验证请求过来的TOKEN
            var userName = _memoryCache.Get(model.Token);
            if (model.NameKey != userName.ToString()) return JwtTokenResponseModel.Error("非法请求");

            //先验证缓存AccessToken
            var cacheKey = $"CacheAccessToken:{model.NameKey}";
            var jwtTokenModel = _memoryCache.Get<JwtTokenUserModel>(cacheKey);
            if (jwtTokenModel != null)
            {
                //如果是生产环境，启用签名
                if (_configuration["IsProduceEnv"].ToBool())
                {
                    //验证签名
                    if (!model.ValidateSign(jwtTokenModel.Pwd))
                    {
                        return JwtTokenResponseModel.Error("签名错误");
                    }
                }

                return jwtTokenModel.TokenResponse;
            }

            //定义Token的有效期
            var expTime = model.Timestamp.ToDateTime();

            //生成User类型的Token
            var findUser = await _sqlSugarClient.Queryable<LoginUser>().With(SqlWith.NoLock)
                .Where(x => x.Status == (int)UniversalStatus.Enable && x.LoginName == model.NameKey).FirstAsync();
            if (findUser == null) return JwtTokenResponseModel.Error("用户不存在");

            jwtTokenModel = new JwtTokenUserModel
            {
                Name = findUser.LoginName,
                Sub = findUser.ID.ToString(),
                Pwd = findUser.Password,
                Role = findUser.IsAdmin ? "Admin" : "User"
            };

            //如果是生产环境，启用签名
            if (_configuration["IsProduceEnv"].ToBool())
            {
                //验证签名
                if (!model.ValidateSign(findUser.Password))
                {
                    return JwtTokenResponseModel.Error("签名错误");
                }
            }

            //生成Token
            var token = _jwtService.CreateToken(jwtTokenModel, expTime);
            jwtTokenModel.TokenResponse = JwtTokenResponseModel.Ok(token, expTime);

            //缓存起来
            var cacheData = jwtTokenModel.ToJson();
            _memoryCache.Set(cacheKey, cacheData, expTime - DateTime.Now);
            _memoryCache.Set(jwtTokenModel.TokenResponse.AccessToken, cacheData, expTime - DateTime.Now);
            return jwtTokenModel.TokenResponse;
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        [HttpGet("Validate")]
        [Authorize]
        public ExecuteResult<JwtTokenUserModel> Validate()
        {
            //token
            string token = string.Empty;
            string authorization = Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization))
            {
                // 必须为 Bearer 认证方案
                if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    // 赋值token
                    token = authorization["Bearer ".Length..].Trim();
                }
            }

            if (!string.IsNullOrEmpty(token))
            {
                var obj = _memoryCache.Get<JwtTokenUserModel>(token);
                if (obj != null)
                {
                    return ExecuteResult<JwtTokenUserModel>.Ok(obj, "验证成功", "200");
                }
            }
            return ExecuteResult<JwtTokenUserModel>.Error("验证成功", "404");
        }
    }
}
