using Dapper;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.WriteOffTask
{
    public class JobService
    {
        private static readonly string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 銷帳
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public bool UpdatePayment(Payment payment)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        // 查出該筆繳款資料
                        var paymentInDB = cn.QueryFirstOrDefault<Payment>(@"
                            SELECT * FROM dbo.Payment
                            WHERE PaymentID=@PaymentID",
                            new { PaymentID = payment.PaymentID }, trans);

                        if (paymentInDB == null)
                            throw new Exception($"銷帳查無銀行帳號 {payment.PaymentID}");

                        // 更新繳款資訊
                        cn.Execute(@"
                            UPDATE dbo.Payment
                                SET PayAmount=@PayAmount,
                                PayDate=@PayDate,
                                ModifyDate=GETDATE(),
                                BankLog=@BankLog
                            WHERE FormID=@FormID",
                            new
                            {
                                PayAmount = payment.PayAmount,
                                PayDate = payment.PayDate,
                                BankLog = payment.BankLog,
                                FormID = paymentInDB.FormID
                            }, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"UpdatePayment: {ex.Message}");
                        return false;
                    }
                }
            }
        }
    }
}
