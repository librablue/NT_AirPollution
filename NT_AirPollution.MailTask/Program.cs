using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.MailTask
{
    internal class Program
    {
        protected static readonly string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;

        static void Main(string[] args)
        {
            MailHelper mailHelper = new MailHelper();
            var sendBox = GetSendBox();

            using (var cn = new SqlConnection(connStr))
            {
                foreach (var box in sendBox)
                {
                    try
                    {
                        var mail = new MailModel
                        {
                            Subject = box.Subject,
                            Body = box.Body,
                            To = new List<string> { box.Address },
                            Attachment = box.Attachment?.Split(';').ToList()
                        };
                        mailHelper.SendMail(mail);

                        // 更新寄送狀態
                        box.SendDate = DateTime.Now;
                        cn.Update(box);
                    }
                    catch (Exception ex)
                    {
                        // 記錄失敗次數
                        box.FailTimes += 1;
                        cn.Update(box);
                    }
                }
            }
        }

        /// <summary>
        /// 取得未寄送的清單，只撈失敗次數5次內
        /// </summary>
        /// <returns></returns>
        static List<SendBox> GetSendBox()
        {
            using (var cn = new SqlConnection(connStr))
            {
                return cn.Query<SendBox>(@"SELECT * FROM SendBox WHERE SendDate IS NULL AND FailTimes<=5").ToList();
            }
        }
    }
}
