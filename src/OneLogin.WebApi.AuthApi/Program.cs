using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using System.Reflection;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using OneLogin.WebApi.AuthApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(configure =>
{
    configure.JsonSerializerOptions.PropertyNamingPolicy = null;
});

//NLog
builder.Logging.AddNLogWeb();

//cors
builder.Services.AddCors(options => options.AddDefaultPolicy(o =>
{
    o.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
}));
builder.Services.AddScoped<IAccessTokenService, AccessTokenService>();
builder.Services.AddScoped<JwtSecurityTokenHandler>();

//swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var security = new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Id = "Bearer",
                Type = ReferenceType.SecurityScheme
            }
        }, Array.Empty<string>() }
    };
    options.AddSecurityRequirement(security);//���һ�������ȫ�ְ�ȫ��Ϣ����AddSecurityDefinition����ָ���ķ�������Ҫһ�£�������Bearer��
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"Authorization: Bearer {token}\"",
        Name = "Authorization",//jwtĬ�ϵĲ�������
        In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
        Type = SecuritySchemeType.ApiKey
    });
    //
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    if (File.Exists(xmlFilename))
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

//AddAuthentication
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.RequireHttpsMetadata = false;
    option.SaveToken = true;
    //
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, //�Ƿ���֤Issuer�������issuer����
        ValidIssuer = builder.Configuration["AuthSettings:Issuer"], //������Issuer
        ValidateAudience = true, //�Ƿ���֤Audience
        ValidAudience = builder.Configuration["AuthSettings:Audience"], //������Audience
        ValidateIssuerSigningKey = true, //�Ƿ���֤SecurityKey
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AuthSettings:SecretKey"])), //SecurityKey
        ValidateLifetime = true, //�Ƿ���֤ʧЧʱ��
        ClockSkew = TimeSpan.FromSeconds(30), //����ʱ���ݴ�ֵ�������������ʱ�䲻ͬ�����⣨�룩
        RequireExpirationTime = true
    };
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

//cors
app.UseCors();

//swagger
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();