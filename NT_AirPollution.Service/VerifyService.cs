using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class VerifyService : BaseService
    {
        /// <summary>
        /// 新增驗證碼記錄
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool AddVerifyLog(VerifyLog log)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(log);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddVerifyLog: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 註冊檢查驗證碼
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public VerifyLog CheckVerifyLog(string email)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<VerifyLog>(@"
                    SELECT TOP 1 * FROM VerifyLog
                    WHERE Email=@Email
                        AND ActiveDate IS NULL
                    ORDER BY CreateDate DESC",
                    new { Email = email });
                return result;
            }
        }

        /// <summary>
        /// 修改驗證碼記錄
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool UpdateVerifyLog(VerifyLog log)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(log);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateVerifyLog: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
