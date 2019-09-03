using System;

namespace Common
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 获取精确到小时的时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetHourDate(this DateTime dateTime)
        {
            var hourDate = dateTime.Date.AddHours(dateTime.Hour);
            return hourDate;
        }

        /// <summary>
        /// 获取精确到天的时间
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime GetDayDate(this DateTime dateTime)
        {
            var hourDate = dateTime.Date;
            return hourDate;
        }
    }
}