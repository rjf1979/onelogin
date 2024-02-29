using System.ComponentModel;

namespace OneLogin.Domains
{
    public enum UniversalStatus:short
    {
        [Description("开启")]
        Enable = 10,
        [Description("关闭")]
        Disable = -10
    }
}
