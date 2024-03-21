using System;

namespace OneLogin.Core
{
    internal static class DateTimeExtension
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
    }
}
