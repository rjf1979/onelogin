using System.IO;
using Newtonsoft.Json;

namespace OneLogin.Core
{
    public static class ObjectExtension
    {

        /// <summary>
        /// 对象序列化成JSON字符串。
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            if (obj.GetType().IsValueType) return obj.ToString();
            return JsonConvert.SerializeObject(obj, JsonSerializerGlobalSettings.Default);
        }

        public static byte[] GetBytes(this Stream stream)
        {
            if (stream == null)
            {
                return null;
            }
            stream.Seek(0, SeekOrigin.Begin);
            byte[] bytes = new byte[stream.Length];
            var _ = stream.Read(bytes, 0, bytes.Length);
            stream.Close();
            return bytes;
        }
    }
}
