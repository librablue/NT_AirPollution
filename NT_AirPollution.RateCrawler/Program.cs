using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NT_AirPollution.RateCrawler
{
    internal class Program
    {
        static async Task Main()
        {
            string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;
            string url = "https://www.etax.nat.gov.tw/etwmain/api/functions/etw160w/interest";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<JObject>(jsonResponse);
                    var currentYear = DateTime.Now.Year - 1911;
                    if (data["intRate"]?[$"{currentYear}"] != null)
                    {
                        using (var cn = new SqlConnection(connStr))
                        {
                            var allRates = cn.GetAll<InterestRate>();
                            var rateArray = data["intRate"][$"{currentYear}"].ToObject<List<JObject>>();
                            foreach (var item in rateArray)
                            {
                                var date = Convert.ToDateTime($"{DateTime.Now.Year}-{item["date"]}");
                                var rate = Convert.ToDouble(item["rate"]);
                                var result = allRates.FirstOrDefault(o => o.Date == date && o.Rate == rate);
                                // 刪除舊資料
                                if (result != null)
                                    cn.Delete(result);

                                // 新增新資料
                                cn.Insert(new InterestRate
                                {
                                    Date = date,
                                    Rate = rate
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"錯誤: {ex.Message}");
            }
        }
    }
}
