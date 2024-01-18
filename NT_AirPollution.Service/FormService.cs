using Aspose.Cells;
using Aspose.Pdf;
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace NT_AirPollution.Service
{
    public class FormService : BaseService
    {
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _paymentPath = ConfigurationManager.AppSettings["Payment"].ToString();

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
                        AND (@CreateUserEmail='' OR CreateUserEmail=@CreateUserEmail)
                        AND (@FormStatus=0 OR FormStatus=@FormStatus)",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        CreateUserEmail = filter.CreateUserEmail ?? "",
                        FormStatus = filter.FormStatus
                    }).ToList();

                foreach (var item in forms)
                {
                    item.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment
                        WHERE FormID=@FormID", new { FormID = item.ID });

                    if (!string.IsNullOrEmpty(item.B_DATE))
                        item.B_DATE2 = base.ChineseDateToWestDate(item.B_DATE);
                    if (!string.IsNullOrEmpty(item.E_DATE))
                        item.E_DATE2 = base.ChineseDateToWestDate(item.E_DATE);
                    if (!string.IsNullOrEmpty(item.S_B_BDATE))
                        item.S_B_BDATE2 = base.ChineseDateToWestDate(item.S_B_BDATE);
                    if (!string.IsNullOrEmpty(item.R_B_BDATE))
                        item.R_B_BDATE2 = base.ChineseDateToWestDate(item.R_B_BDATE);

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();
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
                    result.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                    SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = result.ID });

                    if (!string.IsNullOrEmpty(result.B_DATE))
                        result.B_DATE2 = base.ChineseDateToWestDate(result.B_DATE);
                    if (!string.IsNullOrEmpty(result.E_DATE))
                        result.E_DATE2 = base.ChineseDateToWestDate(result.E_DATE);
                    if (!string.IsNullOrEmpty(result.S_B_BDATE))
                        result.S_B_BDATE2 = base.ChineseDateToWestDate(result.S_B_BDATE);
                    if (!string.IsNullOrEmpty(result.R_B_BDATE))
                        result.R_B_BDATE2 = base.ChineseDateToWestDate(result.R_B_BDATE);

                    result.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();

                    foreach (var sub in result.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    result.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();
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
                        AND (@FormStatus=0 OR FormStatus=@FormStatus)
                        AND C_DATE BETWEEN @StartDate AND @EndDate
                        AND ClientUserID=@ClientUserID",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        PUB_COMP = filter.PUB_COMP,
                        COMP_NAM = filter.COMP_NAM ?? "",
                        CreateUserName = filter.CreateUserName ?? "",
                        FormStatus = filter.FormStatus,
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate.ToString("yyyy-MM-dd 23:59:59"),
                        ClientUserID = filter.ClientUserID
                    }).ToList();

                foreach (var item in result)
                {
                    item.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    if (!string.IsNullOrEmpty(item.B_DATE))
                        item.B_DATE2 = base.ChineseDateToWestDate(item.B_DATE);
                    if (!string.IsNullOrEmpty(item.E_DATE))
                        item.E_DATE2 = base.ChineseDateToWestDate(item.E_DATE);
                    if (!string.IsNullOrEmpty(item.S_B_BDATE))
                        item.S_B_BDATE2 = base.ChineseDateToWestDate(item.S_B_BDATE);
                    if (!string.IsNullOrEmpty(item.R_B_BDATE))
                        item.R_B_BDATE2 = base.ChineseDateToWestDate(item.R_B_BDATE);

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();
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
                    item.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    if (!string.IsNullOrEmpty(item.B_DATE))
                        item.B_DATE2 = base.ChineseDateToWestDate(item.B_DATE);
                    if (!string.IsNullOrEmpty(item.E_DATE))
                        item.E_DATE2 = base.ChineseDateToWestDate(item.E_DATE);
                    if (!string.IsNullOrEmpty(item.S_B_BDATE))
                        item.S_B_BDATE2 = base.ChineseDateToWestDate(item.S_B_BDATE);
                    if (!string.IsNullOrEmpty(item.R_B_BDATE))
                        item.R_B_BDATE2 = base.ChineseDateToWestDate(item.R_B_BDATE);

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();


                    // 檢查今天是否在停復工日期範圍內
                    bool isPause = result.Any(o => o.StopWorks.Any(x => x.DOWN_DATE2 < now && now > x.UP_DATE2));

                    if (isPause)
                        item.WorkStatus = WorkStatus.停工中;
                    else if (now > item.E_DATE2)
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
        public FormView GetFormByUser(Form filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<FormView>(@"
                    SELECT * FROM Form 
                    WHERE CreateUserEmail=@CreateUserEmail
                        AND C_NO=@C_NO",
                    new
                    {
                        CreateUserEmail = filter.CreateUserEmail,
                        C_NO = filter.C_NO
                    });

                if (result != null)
                {
                    result.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                    SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = result.ID });

                    if (!string.IsNullOrEmpty(result.B_DATE))
                        result.B_DATE2 = base.ChineseDateToWestDate(result.B_DATE);
                    if (!string.IsNullOrEmpty(result.E_DATE))
                        result.E_DATE2 = base.ChineseDateToWestDate(result.E_DATE);
                    if (!string.IsNullOrEmpty(result.S_B_BDATE))
                        result.S_B_BDATE2 = base.ChineseDateToWestDate(result.S_B_BDATE);
                    if (!string.IsNullOrEmpty(result.R_B_BDATE))
                        result.R_B_BDATE2 = base.ChineseDateToWestDate(result.R_B_BDATE);

                    result.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();

                    foreach (var sub in result.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    result.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();
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
                    item.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    if (!string.IsNullOrEmpty(item.B_DATE))
                        item.B_DATE2 = base.ChineseDateToWestDate(item.B_DATE);
                    if (!string.IsNullOrEmpty(item.E_DATE))
                        item.E_DATE2 = base.ChineseDateToWestDate(item.E_DATE);
                    if (!string.IsNullOrEmpty(item.S_B_BDATE))
                        item.S_B_BDATE2 = base.ChineseDateToWestDate(item.S_B_BDATE);
                    if (!string.IsNullOrEmpty(item.R_B_BDATE))
                        item.R_B_BDATE2 = base.ChineseDateToWestDate(item.R_B_BDATE);

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    foreach (var sub in item.StopWorks)
                    {
                        sub.DOWN_DATE2 = base.ChineseDateToWestDate(sub.DOWN_DATE);
                        sub.UP_DATE2 = base.ChineseDateToWestDate(sub.UP_DATE);
                    }

                    item.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();
                }

                return result;
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
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        long id = cn.Insert(form, trans);

                        // 附件
                        form.Attachment.FormID = id;
                        cn.Insert(form.Attachment, trans);

                        trans.Commit();
                        return id;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddForm: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
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
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Update(form, trans);

                        // 附件
                        form.Attachment.FormID = form.ID;
                        cn.Update(form.Attachment, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"UpdateForm: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
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
                        Logger.Error($"UpdateStopWork: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 修改付款紀錄
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdatePayment(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        foreach (var item in form.Payments)
                        {
                            item.FormID = form.ID;

                            // ID=0表示新增
                            if (item.ID == 0)
                            {
                                item.CreateDate = DateTime.Now;
                            }
                        }

                        // 清空
                        cn.Execute(@"DELETE FROM Payment WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        // 新增
                        cn.Insert(form.Payments, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"UpdatePayment: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
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
        /// <returns></returns>
        public int CalcTotalMoney(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                form.B_DATE2 = base.ChineseDateToWestDate(form.B_DATE);
                form.E_DATE2 = base.ChineseDateToWestDate(form.E_DATE);
                // 計算施工天數
                double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);
                var diffDays = ((form.E_DATE2 - form.B_DATE2).TotalDays + 1) - downDays;
                var projectCodes = cn.GetAll<ProjectCode>().ToList();
                var projectCode = projectCodes.First(o => o.ID == form.KIND_NO);
                // 基數
                double basicNum = 0;
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
                    case "B":
                        basicNum = form.VOLUMEL.Value;
                        break;
                    case "Z":
                        basicNum = form.MONEY;
                        break;
                }

                if (basicNum >= projectCode.Level1)
                    rate = projectCode.Rate1;
                else if (basicNum * projectCode.Rate3 >= projectCode.Level2)
                    rate = projectCode.Rate2;
                else
                    rate = projectCode.Rate3;

                return Convert.ToInt32(Math.Round(basicNum * rate, 0, MidpointRounding.AwayFromZero));
            }
        }

        /// <summary>
        /// 需補件
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendStatus2(FormView form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Status2.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                String content = sr.ReadToEnd();
                string body = string.Format(content, form.C_NO, form.FailReason.Replace("\n", "<br>"));

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件需補件通知(管制編號 {form.C_NO})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus2: {ex.Message}");
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
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\FormStatus3.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    String content = sr.ReadToEnd();
                    string body = string.Format(content, form.C_NO);
                    string bankAccount = this.GetBankAccount(form.ID.ToString(), form.TotalMoney1);
                    string postAccount = this.GetPostAccount(form.ID.ToString(), form.TotalMoney1);
                    string fileName = $"繳款單{form.C_NO}-{form.SER_NO}({(form.P_KIND == "一次繳清" ? "一次繳清" : "第一期")}).pdf";
                    // 產生繳款單
                    string docPath = this.CreatePaymentPDF(bankAccount, postAccount, fileName, form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費通知(管制編號 {form.C_NO})",
                            Body = body,
                            Attachment = docPath,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus3: {ex.Message}");
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
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\FormStatus4.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                String content = sr.ReadToEnd();
                string body = string.Format(content, form.C_NO);

                // 產生收據
                string pdfTemplateFile = $@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Receipt.html";
                //var fileBytes = this.GeneratePDF(pdfTemplateFile, form);
                // 儲存實體檔案
                string receiptFile = $@"{_paymentPath}\收據{form.C_NO}.pdf";

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費完成(管制編號 {form.C_NO})",
                            Body = body,
                            Attachment = receiptFile,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus4: {ex.Message}");
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
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\CalcStatus3.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    int diffMoney = form.TotalMoney2 - form.TotalMoney1;
                    String content = sr.ReadToEnd();
                    string body = string.Format(content, form.C_NO, diffMoney.ToString("N0"));
                    string bankAccount = this.GetBankAccount(form.ID.ToString(), diffMoney);
                    string postAccount = this.GetPostAccount(form.ID.ToString(), diffMoney);
                    string fileName = $"繳款單{form.C_NO}-{form.SER_NO}(結算補繳).pdf";
                    // 產生繳款單
                    string docPath = this.CreatePaymentPDF(bankAccount, postAccount, fileName, form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件補繳費通知(管制編號 {form.C_NO})",
                            Body = body,
                            Attachment = docPath,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendCalcStatus3: {ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 通過待退費小於4000
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendCalcStatus4(FormView form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\CalcStatus4.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    int diffMoney = form.TotalMoney1 - form.TotalMoney2;
                    String content = sr.ReadToEnd();
                    string body = string.Format(content, form.C_NO, diffMoney.ToString("N0"));
                    string fileName = $"結清證明{form.C_NO}-{form.SER_NO}.pdf";
                    // 產生結清證明
                    string docPath = this.CreateRefundPDF(fileName, form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件退費通知(管制編號 {form.C_NO})",
                            Body = body,
                            Attachment = docPath,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendCalcStatus3: {ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 產生銀行虛擬繳款帳號
        /// </summary>
        /// <param name="autoID">產品自動編號</param>
        /// <param name="price">價格</param>
        /// <returns></returns>
        public string GetBankAccount(string autoID, int price)
        {
            DateTime maxPayDay = Convert.ToDateTime(DateTime.Now.AddDays(6).ToString("yyyy/MM/dd"));
            DateTime firstDayOfYear = Convert.ToDateTime(maxPayDay.ToString("yyyy/01/01"));

            TimeSpan ts = new TimeSpan(maxPayDay.Ticks - firstDayOfYear.Ticks);

            // 代收類別
            string code1_6 = "713587";
            // 到期年末碼
            string code7 = maxPayDay.ToString("yyyy").Substring(3, 1);
            // 到期天(離1/1相差幾天)
            string code8_10 = (ts.TotalDays + 1).ToString().PadLeft(3, '0');
            // 客戶自用編號(產品自動編號5碼)
            if (autoID.Length <= 5)
                autoID = autoID.PadLeft(5, '0');
            else
                autoID = autoID.Substring(autoID.Length - 5, 5);
            string code_11_15 = autoID;
            string code1_15 = code1_6 + code7 + code8_10 + code_11_15;
            string codePrice = price.ToString().PadLeft(10, '0');
            string code1_15Price = code1_15 + codePrice;
            int sum = 0;
            for (int i = 0; i < code1_15Price.Length; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        sum += Convert.ToInt32(code1_15Price[i].ToString()) * 1;
                        break;
                    case 1:
                        sum += Convert.ToInt32(code1_15Price[i].ToString()) * 3;
                        break;
                    case 2:
                        sum += Convert.ToInt32(code1_15Price[i].ToString()) * 7;
                        break;
                }
            }

            // 總和取個位數
            string code16 = (sum % 10).ToString();
            string totalCode = code1_15 + code16;

            return totalCode;
        }

        /// <summary>
        /// 產生郵局虛擬繳款帳號
        /// </summary>
        /// <param name="autoID">產品自動編號</param>
        /// <param name="price">價格</param>
        /// <returns></returns>
        public string GetPostAccount(string autoID, int price)
        {
            DateTime maxPayDay = Convert.ToDateTime(DateTime.Now.AddDays(6).ToString("yyyy/MM/dd"));
            DateTime firstDayOfYear = Convert.ToDateTime(maxPayDay.ToString("yyyy/01/01"));

            TimeSpan ts = new TimeSpan(maxPayDay.Ticks - firstDayOfYear.Ticks);

            // 代收類別
            string code1_6 = "713587";
            // 客戶自用編號(產品自動編號7碼)
            if (autoID.Length <= 7)
                autoID = autoID.PadLeft(7, '0');
            else
                autoID = autoID.Substring(autoID.Length - 7, 7);
            string code_7_13 = autoID;
            string code1_13 = code1_6 + code_7_13;
            string codePrice = price.ToString().PadLeft(10, '0');
            string code1_13Price = code1_13 + codePrice;
            int sum = 0;
            for (int i = 0; i < code1_13Price.Length; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        sum += Convert.ToInt32(code1_13Price[i].ToString()) * 1;
                        break;
                    case 1:
                        sum += Convert.ToInt32(code1_13Price[i].ToString()) * 3;
                        break;
                    case 2:
                        sum += Convert.ToInt32(code1_13Price[i].ToString()) * 7;
                        break;
                }
            }

            // 總和取個位數
            string code14 = (sum % 10 + 1).ToString();
            code14 = code14.Substring(code14.Length - 1, 1);
            string totalCode = code1_13 + code14;

            // 前面加繳款期限
            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            string strMaxPayDay = maxPayDay.ToString("yyMMdd", culture);

            return strMaxPayDay + totalCode;
        }

        /// <summary>
        /// 產生第一段超商條碼
        /// </summary>
        /// <returns></returns>
        public string GetStore1Barcode()
        {
            DateTime maxPayDay = Convert.ToDateTime(DateTime.Now.AddDays(6).ToString("yyyy/MM/dd"));

            CultureInfo culture = new CultureInfo("zh-TW");
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            string strMaxPayDay = maxPayDay.ToString("yyMMdd", culture).Substring(1);
            return strMaxPayDay + "63F";
        }

        /// <summary>
        /// 產生第三段超商條碼
        /// </summary>
        /// <param name="bankAccount">銀行帳號</param>
        /// <param name="priceStr">價格(字串)</param>
        /// <returns></returns>
        public string GetStore3Barcode(string bankAccount, string priceStr)
        {
            //Dictionary<string, int> charTable = new Dictionary<string, int>()
            //{
            //    {"A", 1},{"B", 2},{"C", 3},{"D", 4},{"E", 5},{"F", 6},{"G", 7},{"H", 8},{"I", 9},
            //    {"J", 1},{"K", 2},{"L", 3},{"M", 4},{"N", 5},{"O", 6},{"P", 7},{"Q", 8},{"R", 9},
            //    {"S", 2},{"T", 3},{"U", 4},{"V", 5},{"W", 6},{"X", 7},{"Y", 8},{"Z", 9}
            //};

            string part1 = GetStore1Barcode();
            part1 = part1.Replace("F", "6");
            string part2 = bankAccount;
            string part3 = "0000" + priceStr;

            int part1_sum1 = 0;
            int part1_sum2 = 0;
            int part2_sum1 = 0;
            int part2_sum2 = 0;
            int part3_sum1 = 0;
            int part3_sum2 = 0;
            for (int i = 0; i < part1.Length; i++)
            {
                if (i % 2 == 0)
                    part1_sum1 += Convert.ToInt32(part1[i].ToString());
                else
                    part1_sum2 += Convert.ToInt32(part1[i].ToString());
            }
            for (int i = 0; i < part2.Length; i++)
            {
                if (i % 2 == 0)
                    part2_sum1 += Convert.ToInt32(part2[i].ToString());
                else
                    part2_sum2 += Convert.ToInt32(part2[i].ToString());
            }
            for (int i = 0; i < part3.Length; i++)
            {
                if (i % 2 == 0)
                    part3_sum1 += Convert.ToInt32(part3[i].ToString());
                else
                    part3_sum2 += Convert.ToInt32(part3[i].ToString());
            }

            string checkCode1 = ((part1_sum1 + part2_sum1 + part3_sum1) % 11).ToString();
            string checkCode2 = ((part1_sum2 + part2_sum2 + part3_sum2) % 11).ToString();
            checkCode1 = checkCode1.Replace("10", "B").Replace("0", "A");
            checkCode2 = checkCode2.Replace("10", "Y").Replace("0", "X");

            return "0000" + checkCode1 + checkCode2 + priceStr;
        }

        /// <summary>
        /// 產生繳款單
        /// </summary>
        /// <param name="bankAccount">銀行帳號</param>
        /// <param name="postAccount">郵局帳號</param>
        /// <param name="fileName">產生檔名</param>
        /// <param name="form"></param>
        /// <returns>文件完整路徑</returns>
        public string CreatePaymentPDF(string bankAccount, string postAccount, string fileName, Form form)
        {
            try
            {
                // 如果已經產生過檔案，直接下載
                string existFile = $@"{_paymentPath}\Download\{fileName}";
                if (File.Exists(existFile))
                    return existFile;


                string templatePath = $@"{_paymentPath}\Template\Payment.html";
                string readText = File.ReadAllText(templatePath);
                readText = readText.Replace("#ProjectID#", $"{form.C_NO}-{form.SER_NO}")
                                    .Replace("#PaymentID#", "")
                                    .Replace("#ProjectName#", form.KIND)
                                    .Replace("#ContractID#", form.B_SERNO)
                                    .Replace("#BizCompany#", form.S_NAME)
                                    .Replace("#TotalMoney#", "")
                                    .Replace("#TotalMoneyChinese#", this.GetChineseMoney("") + "整")
                                    .Replace("#CurrentMoney#", "")
                                    .Replace("#BankAccount#", bankAccount)
                                    .Replace("#PayWay#", "一次全繳<br>共分 1 期，本期為第 1 期")
                                    .Replace("#StartDate#", $"民國 {form.B_DATE2.Year - 1911} 年 {form.B_DATE2.Month} 月 {form.B_DATE2.Day} 日");


                Aspose.Pdf.License license = new Aspose.Pdf.License();
                license.SetLicense(HostingEnvironment.MapPath(@"~/license/Aspose.total.lic"));

                Aspose.Pdf.HtmlLoadOptions options = new Aspose.Pdf.HtmlLoadOptions
                {
                    PageInfo =
                {
                    Margin =
                    {
                        Left = 0,
                        Top = 0,
                        Right = 0,
                        Bottom = 0
                    }
                }
                };
                Document pdfDocument = new Document(templatePath, options);
                pdfDocument.Save(existFile);

                return existFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreatePaymentPDF: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生結清證明
        /// </summary>
        /// <param name="fileName">產生檔名</param>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateRefundPDF(string fileName, FormView form)
        {
            try
            {
                // 如果已經產生過檔案，直接下載
                string existFile = $@"{_paymentPath}\Download\{fileName}";
                if (File.Exists(existFile))
                    return existFile;

                string templatePath = $@"{_paymentPath}\Template\結清證明.xlsx";
                var wb = new XLWorkbook(templatePath);
                var ws = wb.Worksheet(0);
                ws.Cell("B2").SetValue(form.COMP_NAM);
                ws.Cell("B3").SetValue(form.C_NO);
                ws.Cell("F3").SetValue($"-{form.SER_NO}");
                ws.Cell("C4").SetValue(form.ADDR);
                ws.Cell("C5").SetValue(form.B_SERNO);
                ws.Cell("C6").SetValue(form.S_NAME);

                int idx = 0;
                // 應繳金額
                string[] payableStrAry = new[] { form.TotalMoney2.ToString() };
                foreach (var item in payableStrAry)
                {
                    ws.Row(9).Cell(16 - idx).SetValue(item);
                    idx -= 2;
                }

                idx = 0;
                // 已繳金額
                int paidAmount = form.Payments.Sum(o => o.Amount);
                string[] paidStrAry = new[] { paidAmount.ToString() };
                foreach (var item in paidStrAry)
                {
                    ws.Row(10).Cell(16 - idx).SetValue(item);
                    idx -= 2;
                }

                idx = 0;
                // 應退金額
                if (form.CalcStatus == CalcStatus.通過待退費小於4000 || form.CalcStatus == CalcStatus.通過待退費大於4000)
                {
                    int diffMoney = form.TotalMoney1 - form.TotalMoney2;
                    string[] diffStrAry = new[] { diffMoney.ToString() };
                    foreach (var item in diffStrAry)
                    {
                        ws.Row(11).Cell(16 - idx).SetValue(item);
                        idx -= 2;
                    }
                }

                idx = 0;
                // 補繳金額
                if (form.CalcStatus == CalcStatus.通過待繳費)
                {
                    int diffMoney = form.TotalMoney2 - form.TotalMoney1;
                    string[] diffStrAry = new[] { diffMoney.ToString() };
                    foreach (var item in diffStrAry)
                    {
                        ws.Row(12).Cell(16 - idx).SetValue(item);
                        idx -= 2;
                    }
                }

                string tmpFile = $@"{_paymentPath}\Download\結清證明{form.C_NO}-{form.SER_NO}.xlsx";
                wb.SaveAs(tmpFile);

                // 轉PDF
                var workbook = new Workbook(tmpFile);
                workbook.Save(existFile);

                return existFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateRefundPDF: {ex.Message}");
                throw ex;
            }
        }
    }
}
