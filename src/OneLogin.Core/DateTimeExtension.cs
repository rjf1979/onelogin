using System;

namespace OneLogin.Core
{
    public static class DateTimeExtension
    {
        /// <summary>
        /// 从1970开始算起的时间戳，单位秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToTimestamp(this DateTime dateTime)
        {
            var jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (int)(dateTime.AddHours(-8) - jan1St1970).TotalSeconds;
        }

        /// <summary>
        /// 从1970开始算起的时间戳，单位毫秒
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long ToTimestampMillisecond(this DateTime dateTime)
        {
            DateTime jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return (long)(dateTime.AddHours(-8) - jan1St1970).TotalMilliseconds;
        }

        /// <summary>
        /// 从1970开始的时间
        /// </summary>
        /// <param name="timeStamp">秒时间戳</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timeStamp)
        {
            var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
            return start.AddSeconds(timeStamp).AddHours(8);
        }

        public static string ToDateTimeString(this DateTime dateTime, string format = "yyyy-MM-dd")
        {
            return dateTime.ToString(format);
        }

        public static string ToDateTimeString(this DateTime? dateTime, string format = "yyyy-MM-dd")
        {
            return dateTime != null ? dateTime.Value.ToString(format) : "";
        }
    }
}
