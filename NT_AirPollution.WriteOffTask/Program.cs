using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.WriteOffTask
{
    internal class Program
    {
        protected static string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;
        private static string _bankFile = ConfigurationManager.AppSettings["BankFile"].ToString();
        private static string _bankFileHistory = ConfigurationManager.AppSettings["BankFileHistory"].ToString();
        private static string notifyMail = ConfigurationManager.AppSettings["NotifyMail"].ToString();
        private static FormService _formService = new FormService();
        private static AccessService _accessService = new AccessService();
        protected static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LoadBankFile();
        }

        /// <summary>
        /// 銷帳
        /// </summary>
        private static void LoadBankFile()
        {
            try
            {
                string[] fileEntries = Directory.GetFiles(_bankFile);
                foreach (string file in fileEntries)
                {
                    string[] line = File.ReadAllLines(file);
                    for (int i = 0; i < line.Length - 1; i++)
                    {
                        string account = line[i].Substring(8, 16);
                        int payAmount = Convert.ToInt32(line[i].Substring(100, 10));
                        DateTime payDate = Convert.ToDateTime($"{2011 + Convert.ToInt32(line[i].Substring(93, 2))}-{line[i].Substring(95, 2)}-{line[i].Substring(97, 2)}");
                        // 取得SQL付款資訊
                        var paymentsInDB = _formService.GetAllPaymentByPaymentID(account);
                        // 查無付款資訊跳過
                        if (paymentsInDB.Count() == 0)
                            continue;

                        // 真實繳費單號
                        var actualPayment = paymentsInDB.FirstOrDefault(o => o.PaymentID == account);
                        // 最新繳費單號
                        var lastPayment = paymentsInDB.OrderByDescending(o => o.CreateDate).FirstOrDefault();

                        // 取得申請單
                        var form = _formService.GetFormByID(actualPayment.FormID);
                        // 依期數判斷更新哪種狀態
                        if (actualPayment.Term == "01")
                        {
                            form.FormStatus = FormStatus.已繳費完成;
                            form.VerifyStage1 = VerifyStage.複審通過;
                            form.IsMailFormStatus = true;
                        }
                        if (actualPayment.Term == "02")
                        {
                            form.CalcStatus = CalcStatus.繳退費完成;
                            form.VerifyStage2 = VerifyStage.複審通過;
                            form.IsMailCalcStatus = true;
                        }

                        form.FIN_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");

                        // 如果繳費金額=應繳金額才更新成繳費完成
                        if (payAmount == actualPayment.PayableAmount)
                        {
                            // 更新申請單
                            _formService.UpdateForm(form);
                        }

                        // 寄信通知
                        _formService.SendStatusMail(form);
                        // 更新付款資訊
                        actualPayment.PayAmount = payAmount;
                        actualPayment.PayDate = payDate;
                        actualPayment.BankLog = line[i];
                        _formService.UpdatePayment(actualPayment);



                        var abudf_1 = new ABUDF_1
                        {
                            F_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd"),
                            F_AMT = payAmount,
                            PM_DATE = payDate.AddYears(-1911).ToString("yyyMMdd"),
                            A_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd"),
                            M_DATE = DateTime.Now,
                            FLNO = account
                        };

                        // 更新Access
                        _accessService.UpdateABUDF_1(abudf_1, lastPayment.PaymentID);
                    }

                    string fileName = Path.GetFileName(file);
                    if (File.Exists($@"{_bankFileHistory}\{fileName}"))
                        File.Delete($@"{_bankFileHistory}\{fileName}");

                    // 移到歷史資料夾
                    File.Move(file, $@"{_bankFileHistory}\{fileName}");
                    // 寄給環保局人員
                    Send2GovUser($@"{_bankFileHistory}\{fileName}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"{ex.Message} / {ex.StackTrace}");
                return;
            }
        }

        /// <summary>
        /// 寄送銷帳檔給環保局人員
        /// </summary>
        /// <param name="file"></param>
        private static void Send2GovUser(string file)
        {
            string fileName = Path.GetFileName(file);
            using (var cn = new SqlConnection(connStr))
            {
                // 寄件夾
                cn.Insert(new SendBox
                {
                    Address = notifyMail,
                    Subject = $"銷帳檔通知 {fileName}",
                    Body = "",
                    Attachment = file,
                    FailTimes = 0,
                    CreateDate = DateTime.Now
                });
            }
        }
    }
}
