using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.PaymentTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("是否為公共工程 (Y/N): ");
                bool isPublic = Console.ReadLine().Trim().ToUpper() == "Y";

                Console.WriteLine("申請日期 (yyyy-MM-dd): ");
                DateTime applyDate = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("審核日期 (yyyy-MM-dd): ");
                DateTime verifyDate = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("開工日期 (yyyy-MM-dd): ");
                DateTime startDate = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("下載繳費單日期 (yyyy-MM-dd): ");
                DateTime today = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("總金額: ");
                double totalPrice = double.Parse(Console.ReadLine());

                Console.WriteLine("本次繳費金額: ");
                double currentPrice = double.Parse(Console.ReadLine());

                PaymentInfo info = new PaymentInfo
                {
                    IsPublic = isPublic,
                    ApplyDate = applyDate,
                    VerifyDate = verifyDate,
                    StartDate = startDate,
                    Today = today,
                    TotalPrice = totalPrice,
                    CurrentPrice = currentPrice
                };

                PaymentInfo result = CalcPayment(info);
                Console.WriteLine($"\n計算結果:");
                Console.WriteLine($"繳費期限: {result.PayEndDate.ToString("yyyy-MM-dd")}");
                Console.WriteLine($"延遲天數: {result.DelayDays}");
                Console.WriteLine($"滯納金: {result.Penalty}");
                Console.WriteLine($"利息: {result.Interest}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("輸入格式錯誤: " + ex.Message);
            }

            Console.ReadLine();
        }

        /// <summary>
        /// 物件深拷貝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepCopy<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// 計算繳費相關資訊
        /// </summary>
        /// <param name="info"></param>
        public static PaymentInfo CalcPayment(PaymentInfo info)
        {
            /*
            * 公共工程繳費期限 = 30天
            * 私人工程繳費期限 = 3天
            * 審核通過日在開工日前，那免滯納金就是從開工日開始算3天或30天
            * 開工後申報，皆無免滯納金優惠，無關審核通過日期，皆由開工日隔天至繳費當天來算滯納金&利息
            */

            // 深拷貝
            PaymentInfo result = DeepCopy<PaymentInfo>(info);
            // 繳費期限
            int payDays = result.IsPublic ? 30 - 1 : 3 - 1;
            // 今天日期
            DateTime today = info.Today;
            result.Rate = 2;

            // 申報日 <= 開工日
            if (result.ApplyDate <= result.StartDate)
            {
                // 審核日 <= 開工日
                if (result.VerifyDate <= result.StartDate)
                {
                    // 開工日加 3 or 30 天
                    result.PayEndDate = result.StartDate.AddDays(payDays);
                }
                else
                {
                    // 審核日加 3 or 30 天
                    result.PayEndDate = result.VerifyDate.AddDays(payDays);
                }

                if (today > result.PayEndDate)
                {
                    // 延遲天數 = 今天 - 開工日
                    result.DelayDays = (today - result.StartDate).Days;
                }
            }
            else
            {
                // 繳費期限 = 當天
                result.PayEndDate = today;
                // 延遲天數 = 今天 - 開工日
                result.DelayDays = (today - result.StartDate).Days;
            }

            if (result.DelayDays <= 30)
            {
                // 滯納金－每逾一日按滯納之金額加徵百分之○．五滯納金
                result.Penalty = Math.Round(result.CurrentPrice * 0.005 * result.DelayDays, 0, MidpointRounding.AwayFromZero);
                // 30天內只算滯納金
                result.Interest = 0;
            }
            else
            {
                // 30天內只算滯納金
                // 滯納金－每逾一日按滯納之金額加徵百分之○．五滯納金
                result.Penalty = Math.Round(result.CurrentPrice * 0.005 * 30, 0, MidpointRounding.AwayFromZero);

                // 30天後算利息
                // 106/03 前:(應繳金額+滯納金)*郵局儲匯局定存利率(浮動)*(逾期天數-30)/365
                // 106/03 後:(應繳金額)*郵局儲匯局定存利率(浮動)*(逾期天數-30)/365
                if (DateTime.Now < new DateTime(2016, 3, 1))
                {
                    result.Interest = Math.Round((result.CurrentPrice + result.Penalty) * result.Rate / 100 * (result.DelayDays - 30) / 365, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    result.Interest = Math.Round(result.CurrentPrice * result.Rate / 100 * (result.DelayDays - 30) / 365, 0, MidpointRounding.AwayFromZero);
                }
            }

            return result;
        }
    }
}
