using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToTaiwanDate(this DateTime date) => date.AddYears(-1911).ToString("yyyMMdd");

        /// <summary>
        /// 民國年字串 (七位數，如 1120101) 轉為 西元 DateTime
        /// </summary>
        /// <param name="dt">民國年字串 (格式需為 YYYMMDD)</param>
        /// <returns>DateTime</returns>
        public static DateTime ToWestDate(this string dt)
        {
            if (string.IsNullOrWhiteSpace(dt) || dt.Length < 7)
            {
                throw new ArgumentException("日期格式錯誤，需為七位數民國年 (如：1120101)");
            }

            try
            {
                // 擷取前三位年份、中間兩位月份、最後兩位日期
                int year = int.Parse(dt.Substring(0, 3)) + 1911;
                int month = int.Parse(dt.Substring(3, 2));
                int day = int.Parse(dt.Substring(5, 2));

                return new DateTime(year, month, day);
            }
            catch (Exception)
            {
                throw new FormatException($"無法將字串 '{dt}' 轉換為有效的日期。");
            }
        }
    }
}
