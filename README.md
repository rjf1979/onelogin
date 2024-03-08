OneLogin 使用对接开发教程
<br />
时序图片：
<br />
![1709883848932](https://github.com/rjf1979/onelogin/assets/4926981/05a40f3e-e7cd-41c9-95ae-5523efca0601)

<h1>这个项目结构说明</h1>
<p>
  <ul>
    <li>OneLogin.WebApi.AuthApi 发放授权口令的接口，在部署环境中可以配置成内网IP，这样可以防止暴露在外网</li>
    <li>OneLogin.WebUI.Login 统一登录的主要站点，只负责登录、获取令牌口令和登录后回跳到各个系统平台</li>
    <li>需接入登录的系统后台，参考项目对接：OneLogin.WebUI.DemoAdmin</li>
  </ul>
</p>

<h1>需要调整的几个代码点，如下：</h1>
<p>
  项目：OneLogin.WebUI.Login <br />  控制器：LoginController.cs 关于自定义用户信息代码
  <pre>
//获取token
var requestTokenModel = new RequestTokenModel
{
    //这里的用户信息，可以自定义再进行扩展，也可以去数据库里读取用户的数据信息
    UserInfo = new RequestUserModel
    {
        Id = username,
        Role = "user",
        Name = username
    },
    Nonce = Guid.NewGuid().ToString("N"),
    Sign = "",
    Timestamp = DateTime.Now.ToTimestamp()
};
  </pre>
</p>

<p>
配置文件里，这些可以自定义配置填写
  <pre>
{
    "LoginSettings": 
    {
      "CookieScheme": "sso.kx-code.com",
      "ExpireHour": 12
    },
    "AuthSettings": 
    {
      "AuthApi": "http://api.sso.kx-code.com", //配置授权API地址
      "Issuer": "onelogin", 
      "Audience": "onelogin", 
      "SecretKey": "8c98028024b54978a1eda66353f6779c",
      "ExpiredTime": "7200" //单位：秒 
    }
}
  </pre>
</p>

<p>
  项目：OneLogin.WebApi.AuthApi 注意appsettings.json，里面的配置数据要和统一登录项目一致
</p>
<h2>如果技术问题，可加我微信：</h2>

![image](https://github.com/rjf1979/onelogin/blob/main/sources/img/vx.jpg)

<h2>也可以进群，我们一起学习进步</h2>

![image](https://github.com/rjf1979/onelogin/blob/main/sources/img/vxq.jpg)

