using System;
using System.Security.Cryptography;
using System.Text;

namespace OneLogin.Core
{
    public static class EncryptExtension
    {
        public static string ToMd5(this string input)
        {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "").ToLower();
        }
    }

}
