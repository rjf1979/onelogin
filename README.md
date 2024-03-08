OneLogin 使用对接开发教程
<br />
时序图片：
<br />
![1709883848932](https://github.com/rjf1979/onelogin/assets/4926981/05a40f3e-e7cd-41c9-95ae-5523efca0601)

<p>
  控制器：LoginController.cs 关于自定义用户信息代码
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
