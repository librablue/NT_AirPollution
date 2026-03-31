using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Service.Extensions;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

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
            // 1. 取得檔案列表
            string[] fileEntries;
            try
            {
                fileEntries = Directory.GetFiles(_bankFile);
            }
            catch (Exception ex)
            {
                Logger.Error($"讀取目錄失敗: {ex.Message}");
                return;
            }

            foreach (string file in fileEntries)
            {
                // 用來存放該檔案內所有行的錯誤摘要
                var errorSummary = new System.Text.StringBuilder();

                try
                {
                    string currentFileName = Path.GetFileNameWithoutExtension(file);
                    string[] parts = currentFileName.Split('_');
                    string taiwanDate = DateTime.Now.ToTaiwanDate();
                    if (parts.Length > 1)
                        taiwanDate = parts[1];

                    string[] lines = File.ReadAllLines(file);

                    // 逐行處理資料
                    for (int i = 0; i < lines.Length - 1; i++)
                    {
                        try
                        {
                            // --- 解析資料 ---
                            string account = lines[i].Substring(8, 16);
                            string fdate = lines[i].Substring(24, 6);
                            int payAmount = Convert.ToInt32(lines[i].Substring(100, 10));
                            DateTime payDate = Convert.ToDateTime($"{2011 + Convert.ToInt32(lines[i].Substring(93, 2))}-{lines[i].Substring(95, 2)}-{lines[i].Substring(97, 2)}");

                            // --- 取得 SQL 付款資訊 ---
                            var paymentsInDB = _formService.GetAllPaymentByPaymentID(account);
                            if (paymentsInDB == null || !paymentsInDB.Any())
                            {
                                throw new Exception($"虛擬帳號 {account} 在資料庫中不存在。");
                            }

                            var actualPayment = paymentsInDB.FirstOrDefault(o => o.PaymentID == account);
                            if (actualPayment == null) continue;

                            var lastPayment = paymentsInDB.OrderByDescending(o => o.CreateDate).FirstOrDefault();

                            // --- 更新申請單狀態 ---
                            var form = _formService.GetFormByID(actualPayment.FormID);
                            if (form == null)
                            {
                                throw new Exception($"找不到對應的申請單 (FormID: {actualPayment.FormID})。");
                            }

                            if (actualPayment.Term == "01")
                            {
                                form.FormStatus = FormStatus.已繳費完成;
                                form.VerifyStage1 = VerifyStage.複審通過;
                                form.IsMailFormStatus = true;
                            }
                            else if (actualPayment.Term == "02")
                            {
                                form.CalcStatus = CalcStatus.繳退費完成;
                                form.VerifyStage2 = VerifyStage.複審通過;
                                form.IsMailCalcStatus = true;
                            }

                            _formService.UpdateForm(form);
                            _formService.SendStatusMail(form);

                            // --- 更新付款資訊 ---
                            actualPayment.PayAmount = payAmount;
                            actualPayment.PayDate = payDate;
                            actualPayment.BankLog = lines[i];
                            _formService.UpdatePayment(actualPayment);

                            // --- 更新 ABUDF ---
                            _accessService.UpdateABUDFByColumn(form.C_NO, form.SER_NO.Value, "FIN_DATE", taiwanDate);

                            // --- 更新 ABUDF_1 ---
                            var abudf_1 = new ABUDF_1
                            {
                                F_DATE = fdate,
                                F_AMT = payAmount,
                                PM_DATE = payDate.AddYears(-1911).ToString("yyyMMdd"),
                                A_DATE = taiwanDate,
                                M_DATE = DateTime.Now,
                                FLNO = account
                            };
                            _accessService.UpdateABUDF_1(abudf_1, lastPayment.PaymentID);

                            // --- 計算繳費資訊與寫入 ABUDF_I ---
                            PaymentInfo info = new PaymentInfo
                            {
                                Today = payDate,
                                IsPublic = form.PUB_COMP,
                                StartDate = form.B_DATE.ToWestDate()
                            };

                            if (string.IsNullOrEmpty(form.AP_DATE1))
                            {
                                info.ApplyDate = form.AP_DATE.ToWestDate();
                                info.VerifyDate = form.VerifyDate1.Value;
                                info.TotalPrice = form.S_AMT.Value;
                                info.CurrentPrice = form.P_AMT.Value;
                            }
                            else
                            {
                                info.ApplyDate = form.AP_DATE1.ToWestDate();
                                info.VerifyDate = form.VerifyDate2.Value;
                                info.TotalPrice = form.S_AMT2.Value;
                                info.CurrentPrice = form.S_AMT2.Value - form.P_AMT.Value;
                            }

                            var res = _formService.CalcPayment(info);
                            if (!string.IsNullOrEmpty(form.AP_DATE1))
                            {
                                res.Interest = 0;
                                res.Penalty = 0;
                            }

                            double sumPrice = Math.Round(res.CurrentPrice + res.Interest + res.Penalty, 0);

                            if (string.IsNullOrEmpty(form.AP_DATE1) && (res.Interest > 0 || res.Penalty > 0))
                            {
                                ABUDF_I abudf_I = new ABUDF_I
                                {
                                    C_NO = form.C_NO,
                                    SER_NO = form.SER_NO,
                                    P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "01" : "02",
                                    S_DATE = res.StartDate.AddDays(res.ApplyDate <= res.StartDate ? 0 : 1).AddYears(-1911).ToString("yyyMMdd"),
                                    E_DATE = payDate.AddYears(-1911).ToString("yyyMMdd"),
                                    PERCENT = res.Rate,
                                    F_AMT = sumPrice,
                                    I_AMT = res.Interest,
                                    PEN_AMT = res.Penalty,
                                    PEN_RATE = res.Penalty > 0 ? 0.5 : (double?)null,
                                    KEYIN = "EPB02",
                                    C_DATE = DateTime.Now,
                                    M_DATE = DateTime.Now
                                };
                                _accessService.AddABUDF_I(abudf_I);
                            }
                        }
                        catch (Exception lineEx)
                        {
                            // 收集錯誤訊息
                            string msg = $"第 {i + 1} 行處理異常: {lineEx.Message}";
                            Logger.Error($"檔案 {file} {msg}");
                            errorSummary.AppendLine(msg);
                        }
                    } // end for

                    // --- 檔案處理完後的搬移動作 ---
                    string fileName = Path.GetFileName(file);
                    string destFile = Path.Combine(_bankFileHistory, fileName);

                    if (File.Exists(destFile))
                        File.Delete(destFile);

                    File.Move(file, destFile);

                    // --- 呼叫政府端方法並傳入錯誤訊息 ---
                    // 如果 errorSummary 為空，代表全部成功
                    string finalErrorMsg = errorSummary.Length > 0 ? errorSummary.ToString() : "OK";
                    Send2GovUser(destFile, finalErrorMsg);
                }
                catch (Exception fileEx)
                {
                    Logger.Error($"處理檔案 {file} 時發生嚴重錯誤: {fileEx.Message}");
                    // 如果需要，也可以針對整個檔案讀取失敗發送通知
                    // Send2GovUser(file, $"檔案讀取失敗: {fileEx.Message}");
                }
            }
        }

        /// <summary>
        /// 寄送銷帳檔給環保局人員
        /// </summary>
        /// <param name="file"></param>
        /// <param name="errorMsg"></param>
        private static void Send2GovUser(string file, string errorMsg)
        {
            string fileName = Path.GetFileName(file);

            // 組合成更友善的郵件內文
            string mailBody = (errorMsg == "OK")
                ? $"檔案 {fileName} 已於 {DateTime.Now:yyyy-MM-dd HH:mm:ss} 處理完成，無異常。"
                : $"檔案 {fileName} 處理過程中發生以下異常：{Environment.NewLine}{errorMsg}";

            using (var cn = new SqlConnection(connStr))
            {
                // 寫入寄件匣資料表
                cn.Insert(new SendBox
                {
                    Address = notifyMail,
                    Subject = $"銷帳檔通知 - {fileName} " + (errorMsg == "OK" ? "(成功)" : "(有異常)"),
                    Body = mailBody, // 放入處理後的錯誤訊息或成功資訊
                    Attachment = file,
                    FailTimes = 0,
                    CreateDate = DateTime.Now
                });
            }
        }
    }
}
