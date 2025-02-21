using Aspose.Cells;
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace NT_AirPollution.Service
{
    public class FormService : BaseService
    {
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"]?.ToString();
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"]?.ToString();
        private readonly string _paymentPath = ConfigurationManager.AppSettings["Payment"]?.ToString();
        private readonly OptionService _optionService = new OptionService();
        private readonly AccessService _accessService = new AccessService();

        /// <summary>
        /// 取得全部表單
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FormView> GetForms(FormFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var forms = cn.Query<FormView>(@"
                    SELECT * FROM Form
                    WHERE (@C_NO='' OR C_NO=@C_NO)
                        AND (@FormStatus=-1 OR FormStatus=@FormStatus)
                        AND (@CalcStatus=-1 OR CalcStatus=@CalcStatus)
                        AND (@VerifyStage1=-1 OR VerifyStage1=@VerifyStage1)
                        AND (@VerifyStage2=-1 OR VerifyStage2=@VerifyStage2)",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        FormStatus = filter.FormStatus,
                        CalcStatus = filter.CalcStatus,
                        VerifyStage1 = filter.VerifyStage1,
                        VerifyStage2 = filter.VerifyStage2
                    }).ToList();

                foreach (var item in forms)
                {
                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }
                }

                return forms;
            }
        }

        /// <summary>
        /// 取得申請單 BY ID
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public FormView GetFormByID(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<FormView>(@"
                    SELECT * FROM Form WHERE ID=@ID",
                    new { ID = id });

                if (result != null)
                {
                    result.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = result.ID });
                    if (result.RefundBank == null) result.RefundBank = new RefundBank();

                    result.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();

                    result.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();

                    foreach (var sub in result.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 取得用戶的申請單
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FormView> GetFormsByUser(FormFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<FormView>(@"
                    SELECT * FROM Form 
                    WHERE (@C_NO='' OR C_NO=@C_NO)
                        AND (@PUB_COMP IS NULL OR PUB_COMP=@PUB_COMP)
                        AND (@COMP_NAM='' OR COMP_NAM LIKE '%'+@COMP_NAM+'%')
                        AND (@CreateUserName='' OR CreateUserName=@CreateUserName)
                        AND C_DATE BETWEEN @StartDate AND @EndDate
                        AND ClientUserID=@ClientUserID",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        PUB_COMP = filter.PUB_COMP,
                        COMP_NAM = filter.COMP_NAM ?? "",
                        CreateUserName = filter.CreateUserName ?? "",
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate.ToString("yyyy-MM-dd 23:59:59"),
                        ClientUserID = filter.ClientUserID
                    }).ToList();

                foreach (var item in result)
                {
                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 取得自主管理的申請單(抓相同統編)
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FormView> GetFormsByCompany(FormFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<FormView>(@"
                    SELECT * FROM Form 
                    WHERE (@C_NO='' OR C_NO=@C_NO)
                        AND (@COMP_NAM='' OR COMP_NAM LIKE '%'+@COMP_NAM+'%')
                        AND (S_G_NO=@CompanyID OR R_G_NO=@CompanyID)",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        COMP_NAM = filter.COMP_NAM ?? "",
                        CompanyID = filter.CompanyID
                    }).ToList();

                var now = DateTime.Now;
                foreach (var item in result)
                {
                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    // 檢查今天是否在停復工日期範圍內
                    bool isPause = result.Any(o => o.StopWorks.Any(x => x.DOWN_DATE2 < now && now > x.UP_DATE2));

                    if (isPause)
                        item.WorkStatus = WorkStatus.停工中;
                    else if (now > base.ChineseDateToWestDate(item.E_DATE))
                        item.WorkStatus = WorkStatus.已完工;
                    else
                        item.WorkStatus = WorkStatus.施工中;
                }

                if (filter.WorkStatus != WorkStatus.全部)
                    result = result.Where(o => o.WorkStatus == filter.WorkStatus).ToList();

                // Todo
                //switch (filter.Commitment)
                //{
                //    case Commitment.未完成認養承諾書:
                //        break;
                //    case Commitment.未完成廢土承諾書:
                //        break;
                //}

                return result;
            }
        }

        /// <summary>
        /// 取得用戶的申請單
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FormView> GetFormByUser(Form filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<FormView>(@"
                    SELECT * FROM Form 
                    WHERE CreateUserEmail=@CreateUserEmail
                        AND C_NO=@C_NO",
                    new
                    {
                        CreateUserEmail = filter.CreateUserEmail,
                        C_NO = filter.C_NO
                    }).ToList();

                foreach (var item in result)
                {
                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 取得申請單 by 管制編號
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<FormView> GetFormsByC_NO(FormFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<FormView>(@"
                    SELECT * FROM Form WHERE C_NO=@C_NO AND ClientUserID=@ClientUserID",
                    new { C_NO = filter.C_NO, ClientUserID = filter.ClientUserID })
                    .OrderBy(o => o.SER_NO).ToList();

                foreach (var item in result)
                {
                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 取得申請表單數
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetFormsCount()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var counter = cn.QuerySingle<long>(@"
                        SELECT COUNT(*) FROM Form");

                return counter;
            }
        }

        /// <summary>
        /// 新增申請單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public long AddForm(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long id = cn.Insert(form);
                    return id;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddForm: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 修改申請單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool UpdateForm(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(form);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateForm: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 刪除申請單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool DeleteForm(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Delete(form);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"DeleteForm: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 取得Payment By帳號
        /// </summary>
        /// <param name="paymentID"></param>
        /// <returns></returns>
        public Payment GetPaymentByPaymentID(string paymentID)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<Payment>(@"
                    SELECT * FROM dbo.Payment WHERE PaymentID=@PaymentID",
                    new { PaymentID = paymentID });

                return result;
            }
        }

        /// <summary>
        /// 新增繳費資料
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddPayment(Payment payment)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Execute(@"DELETE FROM dbo.Payment
                            WHERE FormID=@FormID AND Term=@Term",
                            new
                            {
                                FormID = payment.FormID,
                                Term = payment.Term
                            }, trans);

                        long id = cn.Insert(payment, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddPayment: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

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
                            SELECT * FROM dbo.Payment WHERE PaymentID=@PaymentID",
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
                        Logger.Error($"UpdatePayment: {ex.StackTrace}|{ex.Message}");
                        return false;
                    }
                }
            }
        }

        /// <summary>
        /// 修改停復工
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateStopWork(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in form.StopWorks)
                        {
                            item.FormID = form.ID;
                            item.DOWN_DATE = item.DOWN_DATE2.AddYears(-1911).ToString("yyyMMdd");
                            item.UP_DATE = item.UP_DATE2.AddYears(-1911).ToString("yyyMMdd");
                            item.DOWN_DAY = (item.UP_DATE2 - item.DOWN_DATE2).TotalDays + 1;

                            // ID=0表示新增
                            if (item.ID == 0)
                            {
                                item.C_DATE = DateTime.Now;
                                item.M_DATE = DateTime.Now;
                            }
                        }

                        // 清空
                        cn.Execute(@"DELETE FROM StopWork WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        // 新增
                        cn.Insert(form.StopWorks, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"UpdateStopWork: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 新增退費帳戶
        /// </summary>
        /// <param name="bank"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateRefundBank(RefundBank bank)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    var bankInDB = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = bank.FormID });

                    if (bankInDB == null)
                        cn.Insert(bank);
                    else
                        cn.Update(bank);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateRefundBank: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 新增補繳費證明
        /// </summary>
        /// <param name="proof"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdatePaymentProof(PaymentProof proof)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    var proofInDB = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = proof.FormID });

                    if (proofInDB == null)
                        cn.Insert(proof);
                    else
                        cn.Update(proof);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdatePaymentProof: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 數字轉換為中文
        /// </summary>
        /// <param name="inputNum"></param>
        /// <returns></returns>
        private string GetChineseMoney(string inputNum)
        {
            string[] intArr = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", };
            string[] strArr = { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖", };
            //string[] Chinese = { "", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億" };
            //金額
            string[] Chinese = { "元", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億" };
            char[] tmpArr = inputNum.ToString().ToArray();
            string tmpVal = "";
            for (int i = 0; i < tmpArr.Length; i++)
            {
                tmpVal += strArr[tmpArr[i] - 48];//ASCII編碼 0為48
                tmpVal += Chinese[tmpArr.Length - 1 - i];//根據對應的位數插入對應的單位
            }

            return tmpVal;
        }

        /// <summary>
        /// 計算總金額
        /// </summary>
        /// <param name="form"></param>
        /// <param name="downDays">停工天數</param>
        /// <returns></returns>
        public CalcMoneyResult CalcTotalMoney(FormView form, double downDays)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var diffDays = ((base.ChineseDateToWestDate(form.E_DATE) - base.ChineseDateToWestDate(form.B_DATE)).TotalDays + 1) - downDays;
                var projectCodes = cn.GetAll<ProjectCode>().ToList();
                var projectCode = projectCodes.First(o => o.ID == form.KIND_NO);
                // 基數
                double basicNum = 0;
                // 級數
                int level = 0;
                // 級數文字
                string levelStr = "";
                // 費率
                double rate = 0;
                switch (form.KIND_NO)
                {
                    case "1":
                    case "2":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                    case "A":
                        basicNum = form.AREA.Value * diffDays / 30;
                        break;
                    case "3":
                        basicNum = form.AREA2.Value;
                        break;
                    case "B":
                        basicNum = form.VOLUMEL.Value;
                        break;
                    case "Z":
                        basicNum = form.MONEY;
                        break;
                }

                if (basicNum >= projectCode.Level1)
                {
                    level = 1;
                    levelStr = "第一級";
                    rate = projectCode.Rate1;
                }
                else if (basicNum * projectCode.Rate3 >= projectCode.Level2)
                {
                    level = 2;
                    levelStr = "第二級";
                    rate = projectCode.Rate2;
                }
                else
                {
                    level = 3;
                    levelStr = "第三級";
                    rate = projectCode.Rate3;
                }

                var result = new CalcMoneyResult
                {
                    Level = levelStr,
                    Rate = rate,
                    TotalMoney = Convert.ToDouble(Math.Round(basicNum * rate, 0, MidpointRounding.AwayFromZero))
                };

                return result;
            }
        }

        /// <summary>
        /// 寄送郵件通知
        /// </summary>
        /// <param name="form"></param>
        public void SendStatusMail(FormView form)
        {
            // 是否寄送通知郵件
            if (form.IsMailFormStatus)
            {
                switch (form.FormStatus)
                {
                    case FormStatus.待補件:
                        this.SendFormStatus2(form);
                        break;
                    case FormStatus.通過待繳費:
                        this.SendFormStatus3(form);
                        break;
                    case FormStatus.已繳費完成:
                        this.SendFormStatus4(form);
                        break;
                    case FormStatus.免繳費:
                        this.SendFormStatus5(form);
                        break;
                }
            }

            // 是否寄送通知郵件
            if (form.IsMailCalcStatus)
            {
                switch (form.CalcStatus)
                {
                    case CalcStatus.待補件:
                        this.SendCalcStatus2(form);
                        break;
                    case CalcStatus.通過待繳費:
                        this.SendCalcStatus3(form);
                        break;
                    case CalcStatus.通過待退費小於4000:
                    case CalcStatus.通過待退費大於4000:
                        this.SendCalcStatus45(form);
                        break;
                    case CalcStatus.繳退費完成:
                        this.SendCalcStatus6(form);
                        break;
                }
            }
        }

        /// <summary>
        /// 待補件
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendFormStatus2(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\Status2.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                string content = sr.ReadToEnd();
                string body = string.Format(content, form.COMP_NAM, form.FailReason1.Replace("\n", "<br>"));

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件待補件通知(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus2: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 通過待繳費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendFormStatus3(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\FormStatus3.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費通知(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus3: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 已繳費完成
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendFormStatus4(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\FormStatus4.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                string content = sr.ReadToEnd();
                string body = string.Format(content, form.COMP_NAM);

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費完成(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus4: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 免繳費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendFormStatus5(FormView form)
        {
            int status = 5;
            if (form.S_AMT > 0 && form.S_AMT <= 100) status = 6;
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\FormStatus{status}.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                string content = sr.ReadToEnd();
                string body = string.Format(content, form.COMP_NAM);

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件免繳費(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus5: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 結算待補件
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendCalcStatus2(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\Status2.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                string content = sr.ReadToEnd();
                string body = string.Format(content, form.COMP_NAM, form.FailReason2.Replace("\n", "<br>"));

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件結算通知(待補件)(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus2: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 結算通過補繳費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendCalcStatus3(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\CalcStatus3.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件結算通知(需補繳)(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendCalcStatus3: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 通過待退費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendCalcStatus45(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\CalcStatus{(int)form.CalcStatus}.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    // 產生結清證明
                    string pdfPath = this.CreateClearProofPDF(form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件結算通知(可退費)(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            Attachment = pdfPath,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendCalcStatus45: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 結算通過不需補退費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendCalcStatus6(FormView form)
        {
            string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}\App_Data\Template\CalcStatus6.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    // 產生結清證明
                    string pdfPath = this.CreateClearProofPDF(form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件結算通知(已結清)(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            Attachment = pdfPath,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendCalcStatus6: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 產生繳款單
        /// </summary>
        /// <param name="fileName">產生檔名</param>
        /// <param name="form"></param>
        /// <returns>文件完整路徑</returns>
        public string CreatePaymentPDF(string fileName, Form form)
        {
            try
            {
                string templateFile = $@"{_paymentPath}\Template\Payment.xlsx";
                string tempFile = $@"{_paymentPath}\Download\{fileName}.xlsx";
                string pdfFile = $@"{_paymentPath}\Download\{fileName}.pdf";

                PaymentInfo info = new PaymentInfo
                {
                    IsPublic = form.PUB_COMP,
                    StartDate = this.ChineseDateToWestDate(form.B_DATE),
                };

                // 申報
                if (string.IsNullOrEmpty(form.AP_DATE1))
                {
                    info.ApplyDate = this.ChineseDateToWestDate(form.AP_DATE);
                    info.VerifyDate = form.VerifyDate1.Value;
                    info.TotalPrice = form.S_AMT.Value;
                    info.CurrentPrice = form.P_AMT.Value;
                }
                // 結算
                else
                {
                    info.ApplyDate = this.ChineseDateToWestDate(form.AP_DATE1);
                    info.VerifyDate = form.VerifyDate2.Value;
                    info.TotalPrice = form.S_AMT2.Value;
                    info.CurrentPrice = form.S_AMT2.Value - form.P_AMT.Value;
                }

                // 計算繳費資訊
                var res = CalcPayment(info);

                // 填發日期(申報日)
                DateTime pdate = this.ChineseDateToWestDate(form.AP_DATE);


                double sumPrice = Math.Round(res.CurrentPrice + res.Interest + res.Penalty, 0);
                ABUDF_1 abudf_1 = _accessService.GetABUDF_1(form);
                string transNo = ((abudf_1?.FLNO?.Length == 16) ? abudf_1?.FLNO?.Substring(10, 6) : "000000");
                if (abudf_1 == null)
                {
                    abudf_1 = new ABUDF_1();
                    abudf_1.C_NO = form.C_NO;
                    abudf_1.SER_NO = form.SER_NO;
                    abudf_1.P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "01" : "02";
                    abudf_1.P_DATE = pdate.AddYears(-1911).ToString("yyyMMdd");
                    abudf_1.E_DATE = res.PayEndDate.AddYears(-1911).ToString("yyyMMdd");

                    // 取得聯單序號
                    if (transNo.Length < 16 || !transNo.StartsWith(base.botCode))
                        transNo = _accessService.GetFLNo(pdate.AddYears(-1911).ToString("yyyMMdd"));

                    abudf_1.FLNO = BotHelper.GetPayNo(transNo, sumPrice.ToString(), abudf_1.E_DATE);
                    abudf_1.F_AMT = sumPrice;
                    abudf_1.B_AMT = 0;
                    abudf_1.KEYIN = "EPB02";
                    abudf_1.C_DATE = DateTime.Now;
                    abudf_1.M_DATE = DateTime.Now;
                    // 寫入 ABUDF_1
                    _accessService.AddABUDF_1(abudf_1);
                }

                #region 寫入ABUDF_I
                ABUDF_I abudf_I = new ABUDF_I();
                abudf_I.C_NO = form.C_NO;
                abudf_I.SER_NO = form.SER_NO;
                abudf_I.P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "01" : "02";
                if (res.DelayDays > 0)
                {
                    abudf_I.S_DATE = res.PayEndDate.AddDays(1).AddYears(-1911).ToString("yyyMMdd");
                    abudf_I.E_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                }
                abudf_I.PERCENT = res.Rate;
                abudf_I.F_AMT = sumPrice;
                abudf_I.I_AMT = res.Interest;
                abudf_I.PEN_AMT = res.Penalty;
                abudf_I.PEN_RATE = 0.5;
                abudf_I.KEYIN = "EPB02";
                abudf_I.C_DATE = DateTime.Now;
                abudf_I.M_DATE = DateTime.Now;
                _accessService.AddABUDF_I(abudf_I);
                #endregion

                #region 寫入Payment
                this.AddPayment(new Payment
                {
                    FormID = form.ID,
                    Term = abudf_1.P_TIME,
                    PayEndDate = res.PayEndDate,
                    PaymentID = abudf_1.FLNO,
                    PayableAmount = sumPrice,
                    Penalty = res.Penalty,
                    Interest = res.Interest,
                    Percent = res.Rate,
                    CreateDate = DateTime.Now
                });
                #endregion


                string barcodeMarketA = BotHelper.GetMarketNo(abudf_1.E_DATE);
                string barcodeMarketB = abudf_1.FLNO;
                string barcodeMarketC = BotHelper.GetMarketAmt("0032", sumPrice.ToString(), abudf_1.FLNO, abudf_1.E_DATE);
                string barcodePostA = "19834251";
                string barcodePostB = BotHelper.GetPostNo(transNo, abudf_1.F_AMT.ToString(), abudf_1.E_DATE);
                string barcodePostC = BotHelper.GetPostAmt(abudf_1.F_AMT.ToString());

                var wb = new XLWorkbook(templateFile);
                var ws = wb.Worksheet(1);
                ws.Cell("B2").SetValue(ws.Cell("B2").GetText().Replace("#VerifyDate#", pdate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("M2").SetValue(ws.Cell("M2").GetText().Replace("#VerifyDate#", pdate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("O3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("D4").SetValue(form.COMP_NAM);
                ws.Cell("O4").SetValue(form.S_NAME);
                ws.Cell("D5").SetValue(form.S_NAME);
                ws.Cell("O5").SetValue(form.COMP_NAM);
                ws.Cell("F2").SetValue(ws.Cell("F2").GetText().Replace("#PAY_NO#", barcodeMarketB));
                ws.Cell("F6").SetValue(form.B_SERNO);
                ws.Cell("D7").SetValue(form.P_KIND);
                ws.Cell("F7").SetValue(ws.Cell("F7").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("O7").SetValue(ws.Cell("O7").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("D8").SetValue(ws.Cell("D8").GetText().Replace("#PayEndDate#", res.PayEndDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("O8").SetValue(res.TotalPrice.ToString("N0"));
                ws.Cell("D9").SetValue(res.CurrentPrice.ToString("N0"));
                ws.Cell("O9").SetValue(this.GetChineseMoney(res.TotalPrice.ToString()));
                ws.Cell("D10").SetValue(res.Penalty.ToString("N0"));
                ws.Cell("D11").SetValue(res.Interest.ToString("N0"));
                ws.Cell("D12").SetValue(sumPrice.ToString("N0"));
                ws.Cell("M12").SetValue(form.B_SERNO);
                ws.Cell("D13").SetValue(this.GetChineseMoney(sumPrice.ToString()));
                ws.Cell("B17").SetValue(ws.Cell("B17").GetText().Replace("#VerifyDate#", pdate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("M17").SetValue(ws.Cell("M17").GetText().Replace("#PAY_NO#", barcodeMarketB));
                ws.Cell("D18").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("I18").SetValue(form.COMP_NAM);
                ws.Cell("D19").SetValue(form.S_NAME);
                ws.Cell("P19").SetValue(form.B_SERNO);
                ws.Cell("D20").SetValue(form.P_KIND);
                ws.Cell("F20").SetValue(ws.Cell("F20").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("D21").SetValue(ws.Cell("D21").GetText().Replace("#PayEndDate#", res.PayEndDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D22").SetValue(res.CurrentPrice.ToString("N0"));
                ws.Cell("D23").SetValue(res.Penalty.ToString("N0"));
                ws.Cell("I23").SetValue(res.Interest.ToString("N0"));
                ws.Cell("D24").SetValue(sumPrice.ToString("N0"));
                ws.Cell("D25").SetValue(this.GetChineseMoney(sumPrice.ToString()));
                ws.Cell("B29").SetValue(ws.Cell("B29").GetText().Replace("#VerifyDate#", pdate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D30").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("I30").SetValue(form.COMP_NAM);
                ws.Cell("D31").SetValue(form.S_NAME);
                ws.Cell("P31").SetValue(form.B_SERNO);
                ws.Cell("D32").SetValue(form.P_KIND);
                ws.Cell("F32").SetValue(ws.Cell("F32").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("O32").SetValue(res.PayEndDate.AddYears(-1911).ToString("yyy年MM月dd日"));
                ws.Cell("D34").SetValue(res.CurrentPrice.ToString("N0"));
                ws.Cell("I34").SetValue(res.Penalty.ToString("N0"));
                ws.Cell("O34").SetValue(res.Interest.ToString("N0"));
                ws.Cell("D35").SetValue(sumPrice.ToString("N0"));
                ws.Cell("G35").SetValue(ws.Cell("G35").GetText().Replace("#F_AMTC#", this.GetChineseMoney(sumPrice.ToString())));
                ws.Cell("I36").SetValue(barcodeMarketB);
                ws.Cell("C37").SetValue($"*{abudf_1.FLNO}*");
                ws.Cell("K37").SetValue($"*{barcodeMarketA}*");
                ws.Cell("K38").SetValue(barcodeMarketA);
                ws.Cell("K39").SetValue($"*{barcodeMarketB}*");
                ws.Cell("K40").SetValue(barcodeMarketB);
                ws.Cell("K41").SetValue($"*{barcodeMarketC}*");
                ws.Cell("K42").SetValue(barcodeMarketC);
                ws.Cell("K45").SetValue($"*{barcodePostB}*");
                ws.Cell("K46").SetValue(barcodePostB);
                ws.Cell("K47").SetValue($"*{barcodePostC}*");
                ws.Cell("K48").SetValue(barcodePostC);
                wb.SaveAs(tempFile);

                // 轉PDF
                Aspose.Cells.License license = new Aspose.Cells.License();
                license.SetLicense(HostingEnvironment.MapPath(@"~/license/Aspose.total.lic"));
                var workbook = new Aspose.Cells.Workbook(tempFile);
                foreach (Aspose.Cells.Worksheet worksheet in workbook.Worksheets)
                {
                    Aspose.Cells.PageSetup pageSetup = worksheet.PageSetup;
                    pageSetup.TopMargin = 1;
                    pageSetup.RightMargin = 0;
                    pageSetup.BottomMargin = 1;
                    pageSetup.LeftMargin = 0;
                    pageSetup.FitToPagesWide = 1;
                    pageSetup.CenterHorizontally = true;
                    pageSetup.Zoom = 90;
                    pageSetup.PaperSize = PaperSizeType.PaperA4;
                }

                FontConfigs.SetFontFolder($@"{_paymentPath}\Template", false);
                workbook.Save(pdfFile);
                return pdfFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreatePaymentPDF: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生結清證明
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateClearProofPDF(FormView form)
        {
            try
            {
                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\結清證明.xlsx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{form.C_NO}-{form.SER_NO}結清證明.pdf";

                var wb = new XLWorkbook(templateFile);
                var ws = wb.Worksheet(1);
                ws.Cell("B2").SetValue(form.COMP_NAM);
                ws.Cell("B3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("C4").SetValue(form.ADDR);
                ws.Cell("C5").SetValue(form.B_SERNO);
                ws.Cell("C6").SetValue(form.S_NAME);
                ws.Cell("E28").SetValue(DateTime.Now.AddYears(-1911).ToString("yyy"));
                ws.Cell("I28").SetValue(DateTime.Now.ToString("MM"));
                ws.Cell("M28").SetValue(DateTime.Now.ToString("dd"));

                int idx = 0;
                // 應繳金額
                foreach (char item in form.S_AMT2.ToString().Reverse())
                {
                    ws.Row(9).Cell(16 - idx).SetValue(item.ToString());
                    idx += 2;
                }

                idx = 0;
                // 已繳金額
                double paidAmount = form.Payments.Sum(o => o.PayAmount ?? 0);
                foreach (char item in paidAmount.ToString().Reverse())
                {
                    ws.Row(10).Cell(16 - idx).SetValue(item.ToString());
                    idx += 2;
                }

                idx = 0;
                // 應退金額
                if (form.CalcStatus == CalcStatus.通過待退費小於4000 || form.CalcStatus == CalcStatus.通過待退費大於4000)
                {
                    double diffMoney = Math.Abs(paidAmount - form.S_AMT2.Value);
                    foreach (char item in diffMoney.ToString().Reverse())
                    {
                        ws.Row(11).Cell(16 - idx).SetValue(item.ToString());
                        idx += 2;
                    }
                }

                idx = 0;
                // 補繳金額
                if (form.CalcStatus == CalcStatus.通過待繳費)
                {
                    double diffMoney = Math.Abs(form.S_AMT2.Value - paidAmount);
                    foreach (char item in diffMoney.ToString().Reverse())
                    {
                        ws.Row(12).Cell(16 - idx).SetValue(item.ToString());
                        idx += 2;
                    }
                }

                string tempFile = $@"{_paymentPath}\Download\結清證明{form.C_NO}-{form.SER_NO}.xlsx";
                wb.SaveAs(tempFile);

                // 轉PDF
                Aspose.Cells.License license = new Aspose.Cells.License();
                license.SetLicense(HostingEnvironment.MapPath(@"~/license/Aspose.total.lic"));
                var workbook = new Aspose.Cells.Workbook(tempFile);
                foreach (Aspose.Cells.Worksheet worksheet in workbook.Worksheets)
                {
                    worksheet.PageSetup.FitToPagesWide = 1;
                }
                workbook.Save(resultFile);

                return resultFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateClearProofPDF: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生免徵證明
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateFreeProofPDF(FormView form)
        {
            try
            {
                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\免徵證明.xlsx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{form.C_NO}-{form.SER_NO}免徵證明.pdf";

                var wb = new XLWorkbook(templateFile);
                var ws = wb.Worksheet(1);
                ws.Cell("C2").SetValue(form.COMP_NAM);
                ws.Cell("C3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("C4").SetValue(form.ADDR);
                ws.Cell("C5").SetValue(form.B_SERNO);
                ws.Cell("C6").SetValue(form.S_NAME);
                ws.Cell("E23").SetValue(DateTime.Now.AddYears(-1911).ToString("yyy"));
                ws.Cell("I23").SetValue(DateTime.Now.ToString("MM"));
                ws.Cell("M23").SetValue(DateTime.Now.ToString("dd"));

                if (form.S_AMT == 0)
                {
                    ws.Cell("C7").SetValue("■");
                }
                else if (form.S_AMT <= 100)
                {
                    ws.Cell("C9").SetValue("■");
                    ws.Cell("D10").SetValue("■");
                }

                string tempFile = $@"{_paymentPath}\Download\免徵證明{form.C_NO}-{form.SER_NO}.xlsx";
                wb.SaveAs(tempFile);

                // 轉PDF
                Aspose.Cells.License license = new Aspose.Cells.License();
                license.SetLicense(HostingEnvironment.MapPath(@"~/license/Aspose.total.lic"));
                var workbook = new Aspose.Cells.Workbook(tempFile);
                foreach (Aspose.Cells.Worksheet worksheet in workbook.Worksheets)
                {
                    worksheet.PageSetup.FitToPagesWide = 1;
                }
                workbook.Save(resultFile);

                return resultFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateFreeProofPDF: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生繳費證明
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreatePaymentProofPDF(FormView form)
        {
            try
            {
                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\繳費證明.docx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{form.C_NO}-{form.SER_NO}繳費證明.pdf";

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense(HostingEnvironment.MapPath(@"~/license/Aspose.total.lic"));

                Aspose.Words.Document doc = new Aspose.Words.Document(templateFile);
                doc.Range.Replace("@S_NAME", form.S_NAME);
                doc.Range.Replace("@COMP_NAM", form.COMP_NAM);
                doc.Range.Replace("@C_NO", $"{form.C_NO}-{form.SER_NO}");
                doc.Range.Replace("@B_SERNO", form.B_SERNO);
                doc.Range.Replace("@S_AMT", form.S_AMT.Value.ToString("N0"));
                var payment = form.Payments.FirstOrDefault(o => o.Term == "01");
                if (payment?.Penalty > 0)
                    doc.Range.Replace("@Penalty", $"(含滯納金{payment.Penalty}元)");
                else
                    doc.Range.Replace("@Penalty", "");

                doc.Range.Replace("@A_DATE", payment?.PayDate?.ToString("yyyy-MM-dd") ?? "");
                doc.Range.Replace("@Year", DateTime.Now.AddYears(-1911).ToString("yyy"));
                doc.Range.Replace("@Month", DateTime.Now.AddYears(-1911).ToString("MM"));
                doc.Range.Replace("@Date", DateTime.Now.AddYears(-1911).ToString("dd"));
                doc.Save(resultFile);

                return resultFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateFreeProofPDF: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 計算繳費相關資訊
        /// </summary>
        /// <param name="info"></param>
        public PaymentInfo CalcPayment(PaymentInfo info)
        {
            /*
            * 公共工程繳費期限 = 30天
            * 私人工程繳費期限 = 3天
            * 審核通過日在開工日前，那免滯納金就是從開工日開始算3天或30天
            * 開工後申報，皆無免滯納金優惠，無關審核通過日期，皆由開工日隔天至繳費當天來算滯納金&利息
            */

            // 深拷貝
            PaymentInfo result = base.DeepCopy<PaymentInfo>(info);
            // 繳費期限
            int payDays = result.IsPublic ? 30 - 1 : 3 - 1;
            // 今天日期
            DateTime today = DateTime.Now;


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
                var interestRate = _optionService.GetRates().FirstOrDefault();
                if (interestRate != null)
                    result.Rate = interestRate.Rate;

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
