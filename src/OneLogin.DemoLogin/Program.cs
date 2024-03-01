using Flurl.Http;
using Newtonsoft.Json;
using OneLogin.Core;
using OneLogin.Core.Models;

RequestTokenModel requestToken = new RequestTokenModel
{
    Nonce = Guid.NewGuid().ToString("N"),
    Sign = "",
    Timestamp = DateTime.Now.ToTimestamp(),
    UserInfo = new RequestUserModel
    {
        Id = "1",
        Role = "admin",
        Name = "admin"
    }
};
requestToken.BuildSign("8c98028024b54978a1eda66353f6779c");
var json = JsonConvert.SerializeObject(requestToken);
var url = "http://api.sso.kx-code.com/api/Auth/Authorize";
var response = await url.PostJsonAsync(requestToken);
if (response.StatusCode == 200)
{
    var content = await response.GetStringAsync();
    //DemoResponseTokenModel responseToken = await response.GetJsonAsync<DemoResponseTokenModel>();
    Console.WriteLine(content);
}
Console.ReadKey();