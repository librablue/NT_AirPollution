using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
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
                    string currentFileName = Path.GetFileNameWithoutExtension(file); // 取得檔名，不含副檔名
                    string[] parts = currentFileName.Split('_'); // 用 "_" 分割
                    string chineseToday = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                    if (parts.Length > 1)
                        chineseToday = parts[1]; // 取得檔名後面的日期

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
                            form.FIN_DATE = chineseToday;
                        }

                        // 更新申請單
                        _formService.UpdateForm(form);

                        // 寄信通知
                        _formService.SendStatusMail(form);
                        // 更新付款資訊
                        actualPayment.PayAmount = payAmount;
                        actualPayment.PayDate = payDate;
                        actualPayment.BankLog = line[i];
                        _formService.UpdatePayment(actualPayment);

                        #region 更新ABUDF
                        _accessService.UpdateABUDFByColumn(form.C_NO, form.SER_NO.Value, "FIN_DATE", chineseToday);
                        #endregion

                        #region 更新ABUDF_1
                        var abudf_1 = new ABUDF_1
                        {
                            F_DATE = chineseToday,
                            F_AMT = payAmount,
                            PM_DATE = payDate.AddYears(-1911).ToString("yyyMMdd"),
                            A_DATE = chineseToday,
                            M_DATE = DateTime.Now,
                            FLNO = account
                        };
                        _accessService.UpdateABUDF_1(abudf_1, lastPayment.PaymentID);
                        #endregion

                        #region 計算繳費資訊
                        PaymentInfo info = new PaymentInfo
                        {
                            Today = payDate,
                            IsPublic = form.PUB_COMP,
                            StartDate = _formService.ChineseDateToWestDate(form.B_DATE)
                        };
                        // 申報
                        if (string.IsNullOrEmpty(form.AP_DATE1))
                        {
                            info.ApplyDate = _formService.ChineseDateToWestDate(form.AP_DATE);
                            info.VerifyDate = form.VerifyDate1.Value;
                            info.TotalPrice = form.S_AMT.Value;
                            info.CurrentPrice = form.P_AMT.Value;
                        }
                        // 結算
                        else
                        {
                            info.ApplyDate = _formService.ChineseDateToWestDate(form.AP_DATE1);
                            info.VerifyDate = form.VerifyDate2.Value;
                            info.TotalPrice = form.S_AMT2.Value;
                            info.CurrentPrice = form.S_AMT2.Value - form.P_AMT.Value;
                        }

                        // 計算繳費資訊
                        var res = _formService.CalcPayment(info);
                        // 結算沒有滯納金&利息
                        if (!string.IsNullOrEmpty(form.AP_DATE1))
                        {
                            res.Interest = 0;
                            res.Penalty = 0;
                        }

                        double sumPrice = Math.Round(res.CurrentPrice + res.Interest + res.Penalty, 0);
                        #endregion

                        #region 寫入ABUDF_I
                        // 申報才寫
                        if (string.IsNullOrEmpty(form.AP_DATE1))
                        {
                            ABUDF_I abudf_I = new ABUDF_I();
                            abudf_I.C_NO = form.C_NO;
                            abudf_I.SER_NO = form.SER_NO;
                            abudf_I.P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "01" : "02";
                            // 逾期起始日(逾期:開工日隔天 沒逾期:開工日)
                            abudf_I.S_DATE = res.StartDate.AddDays(res.ApplyDate <= res.StartDate ? 0 : 1).AddYears(-1911).ToString("yyyMMdd");
                            // 逾期結束日是繳費日當天
                            abudf_I.E_DATE = payDate.AddYears(-1911).ToString("yyyMMdd");
                            abudf_I.PERCENT = res.Rate;
                            abudf_I.F_AMT = sumPrice;
                            abudf_I.I_AMT = res.Interest;
                            abudf_I.PEN_AMT = res.Penalty;
                            abudf_I.PEN_RATE = res.Penalty > 0 ? 0.5 : (double?)null;
                            abudf_I.KEYIN = "EPB02";
                            abudf_I.C_DATE = DateTime.Now;
                            abudf_I.M_DATE = DateTime.Now;
                            _accessService.AddABUDF_I(abudf_I);
                        }
                        #endregion
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
