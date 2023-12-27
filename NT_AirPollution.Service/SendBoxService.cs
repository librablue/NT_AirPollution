using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class SendBoxService : BaseService
    {
        /// <summary>
        /// 新增寄信排程
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public long AddSendBox(SendBox box)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long id = cn.Insert(box);
                    return id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 檢查寄信頻率
        /// </summary>
        /// <param name="address">收件地址</param>
        /// <returns></returns>
        public SendBox CheckSendBoxFrequency(string address)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var sendBox = cn.QueryFirstOrDefault<SendBox>(@"
                    SELECT TOP 1 * FROM SendBox WHERE Address=@Address
                        ORDER BY CreateDate DESC",
                    new { Address = address });

                return sendBox;
            }
        }
    }
}
