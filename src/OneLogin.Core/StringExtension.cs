using System;
using System.Security.Cryptography;
using System.Text;

namespace OneLogin.Core
{
    public static class StringExtension
    {
        //public static bool IsEmptyOrNull(this string input)
        //{
        //    return string.IsNullOrEmpty(input?.Trim());
        //}

        //public static byte ToByte(this string input)
        //{
        //    if (byte.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return 0;
        //}

        //public static byte? ToByteOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (byte.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}

        //public static short ToShort(this string input)
        //{
        //    if (short.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return 0;
        //}

        //public static short? ToShortOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (short.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}

        //public static int ToInt(this string input)
        //{
        //    if (int.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return 0;
        //}

        //public static int? ToIntOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (int.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}

        //public static decimal ToDecimal(this string input)
        //{
        //    if (decimal.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return 0m;
        //}

        //public static decimal? ToDecimalOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (decimal.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}

        //public static long ToLong(this string input)
        //{
        //    if (long.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return 0L;
        //}

        //public static long? ToLongOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (long.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}

        //public static bool IsMobile(this string mobile)
        //{
        //    return Regex.IsMatch(mobile, "^134[0-8]\\d{7}$|^13[^4]\\d{8}$|^14[5-9]\\d{8}$|^15[^4]\\d{8}$|^16[6]\\d{8}$|^17[0-8]\\d{8}$|^18[\\d]{9}$|^19[8,9]\\d{8}$");
        //}

        public static bool ToBool(this string input)
        {
            if (string.IsNullOrEmpty(input)) return false;

            if (bool.TryParse(input, out var result))
            {
                return result;
            }

            if (input == "1") return true;
            if (input.Equals("true", StringComparison.CurrentCultureIgnoreCase)) return true;

            return false;
        }

        //public static bool? ToBoolOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (bool.TryParse(input, out var result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}

        //public static DateTime? ToDateTimeOrNull(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return null;
        //    if (DateTime.TryParse(input, out var time))
        //    {
        //        return time;
        //    }

        //    return null;
        //}

        //public static DateTime ToDateTime(this string input)
        //{
        //    if (string.IsNullOrEmpty(input)) return DateTime.MinValue;
        //    if (DateTime.TryParse(input, out var time))
        //    {
        //        return time;
        //    }

        //    return DateTime.MinValue;
        //}

        public static string ToMd5(this string input)
        {
            using var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            var strResult = BitConverter.ToString(result);
            return strResult.Replace("-", "").ToLower();
        }

        //public static string ToHMACSHA256(this string input, string secret)
        //{
        //    return EncryptProvider.HMACSHA256(input, secret).ToLower();
        //}

        //public static string ToHMACSHA512(this string input, string secret)
        //{
        //    return EncryptProvider.HMACSHA512(input, secret).ToLower();
        //}

        //public static T ToObject<T>(this string input) where T : class
        //{
        //    if (string.IsNullOrEmpty(input))
        //    {
        //        return default;
        //    }

        //    try
        //    {
        //        var t = JsonConvert.DeserializeObject<T>(input, JsonSerializerGlobalSettings.Default);
        //        if (t == null)
        //        {
        //            var jObject = JObject.Parse(input);
        //            return jObject.ToObject<T>();
        //        }
        //        return t;
        //    }
        //    catch
        //    {
        //        return default;
        //    }
        //}

        //public static string ToUTF8(this string input,Encoding encoding)
        //{
        //    var bts = encoding.GetBytes(input);
        //    return Encoding.UTF8.GetString(bts);
        //}
    }

}
