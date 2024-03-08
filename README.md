OneLogin 使用对接开发教程
<br />
时序图片：
<br />
![1709883848932](https://github.com/rjf1979/onelogin/assets/4926981/05a40f3e-e7cd-41c9-95ae-5523efca0601)

<h1>需要调整的几个代码点，如下：</h1>
<p>
  项目：OneLogin.WebUI.Login   控制器：LoginController.cs 关于自定义用户信息代码
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
配置文件里，已经注明了那些可以自定义配置填写
  <pre>
{
    "LoginSettings": 
    {
      "CookieScheme": "sso.kx-code.com", //可以自定义CookieScheme
      "ExpireHour": 12
    },
    "AuthSettings": 
    {
      "AuthApi": "http://api.sso.kx-code.com", //配置授权API地址
      "Issuer": "onelogin", //可自定义
      "Audience": "onelogin", //可自定义
      "SecretKey": "8c98028024b54978a1eda66353f6779c",
      "ExpiredTime": "7200" //单位：秒 
    }
}
  </pre>
</p>
