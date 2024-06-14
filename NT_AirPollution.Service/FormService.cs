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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
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
                        AND (@CreateUserEmail='' OR CreateUserEmail=@CreateUserEmail)
                        AND (@FormStatus=-1 OR FormStatus=@FormStatus)
                        AND (@CalcStatus=-1 OR CalcStatus=@CalcStatus)",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        CreateUserEmail = filter.CreateUserEmail ?? "",
                        FormStatus = filter.FormStatus,
                        CalcStatus = filter.CalcStatus
                    }).ToList();

                foreach (var item in forms)
                {
                    item.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.PaymentProof = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.PaymentProof == null) item.PaymentProof = new PaymentProof();

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
                    result.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = result.ID }).ToList();

                    result.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = result.ID });
                    if (result.RefundBank == null) result.RefundBank = new RefundBank();

                    result.PaymentProof = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = result.ID });
                    if (result.PaymentProof == null) result.PaymentProof = new PaymentProof();

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
                    item.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.PaymentProof = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.PaymentProof == null) item.PaymentProof = new PaymentProof();

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
                    item.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.PaymentProof = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.PaymentProof == null) item.PaymentProof = new PaymentProof();

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
                    item.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.PaymentProof = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.PaymentProof == null) item.PaymentProof = new PaymentProof();

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
                    item.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();

                    item.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.RefundBank == null) item.RefundBank = new RefundBank();

                    item.PaymentProof = cn.QueryFirstOrDefault<PaymentProof>(@"
                        SELECT * FROM PaymentProof WHERE FormID=@FormID",
                        new { FormID = item.ID });
                    if (item.PaymentProof == null) item.PaymentProof = new PaymentProof();

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
                        var attachments = form.Attachments.Where(o => !string.IsNullOrEmpty(o.FileName));
                        foreach (var item in attachments)
                        {
                            item.ID = 0;
                            item.FormID = id;
                            item.CreateDate = DateTime.Now;
                        }
                        cn.Insert(attachments, trans);

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
                        cn.Execute(@"DELETE FROM Attachment WHERE FormID=@FormID", new { FormID = form.ID }, trans);
                        var attachments = form.Attachments.Where(o => !string.IsNullOrEmpty(o.FileName));
                        foreach (var item in attachments)
                        {
                            item.ID = 0;
                            item.FormID = form.ID;
                            item.CreateDate = item.CreateDate ?? DateTime.Now;
                        }

                        cn.Insert(attachments, trans);

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
                    Logger.Error($"UpdateRefundBank: {ex.Message}");
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
                    Logger.Error($"UpdatePaymentProof: {ex.Message}");
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
                var diffDays = ((form.E_DATE2 - form.B_DATE2).TotalDays + 1) - downDays;
                var projectCodes = cn.GetAll<ProjectCode>().ToList();
                var projectCode = projectCodes.First(o => o.ID == form.KIND_NO);
                // 基數
                double basicNum = 0;
                // 級數
                int level = 0;
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
                {
                    level = 1;
                    rate = projectCode.Rate1;
                }
                else if (basicNum * projectCode.Rate3 >= projectCode.Level2)
                {
                    level = 2;
                    rate = projectCode.Rate2;
                }
                else
                {
                    level = 3;
                    rate = projectCode.Rate3;
                }

                var result = new CalcMoneyResult
                {
                    Level = level,
                    Rate = rate,
                    TotalMoney = Convert.ToInt32(Math.Round(basicNum * rate, 0, MidpointRounding.AwayFromZero))
                };

                return result;
            }
        }

        /// <summary>
        /// 待補件
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendFormStatus2(FormView form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Status2.txt");
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
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    string fileName = $"繳款單{form.C_NO}-{form.SER_NO}({(form.P_KIND == "一次繳清" ? "一次繳清" : "第一期")})";
                    // 產生繳款單
                    string pdfPath = this.CreatePaymentPDF(fileName, form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費通知(工程名稱 {form.COMP_NAM})",
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
                    Logger.Error($"SendFormStatus4: {ex.Message}");
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
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\FormStatus5.txt");
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
                    Logger.Error($"SendFormStatus5: {ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 暫免繳費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendFormStatus6(FormView form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\FormStatus6.txt");
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
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件暫免繳費(工程名稱 {form.COMP_NAM})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendFormStatus6: {ex.Message}");
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
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Status2.txt");
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
                    Logger.Error($"SendFormStatus2: {ex.Message}");
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
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    string fileName = $"繳款單{form.C_NO}-{form.SER_NO}(結算補繳)";
                    // 產生繳款單
                    string docPath = this.CreatePaymentPDF(fileName, form);

                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件結算通知(需補繳)(工程名稱 {form.COMP_NAM})",
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
        /// 通過待退費
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SendCalcStatus45(FormView form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\CalcStatus{(int)form.CalcStatus}.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    // 產生結清證明
                    string pdfPath = this.CreateProofPDF(form);

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
                    Logger.Error($"SendCalcStatus45: {ex.Message}");
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
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\CalcStatus6.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                try
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, form.COMP_NAM);
                    // 產生結清證明
                    string pdfPath = this.CreateProofPDF(form);

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
                    Logger.Error($"SendCalcStatus6: {ex.Message}");
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

                DateTime verifyDate = string.IsNullOrEmpty(form.AP_DATE1) ? form.VerifyDate1.Value : form.VerifyDate2.Value;
                double totalPrice = string.IsNullOrEmpty(form.AP_DATE1) ? form.S_AMT.Value : form.S_AMT2.Value;
                double currentPrice = string.IsNullOrEmpty(form.AP_DATE1) ? form.P_AMT.Value : (form.S_AMT2.Value - form.P_AMT.Value);

                // 繳款期限：1、2類3天，其他30天
                DateTime payEndDate = verifyDate.AddDays(30);
                if (form.KIND_NO == "1" || form.KIND_NO == "2")
                    payEndDate = verifyDate.AddDays(3);

                // 利息
                double interest = 0;
                // 滯納金
                double delayPrice = 0;
                var delayDays = (DateTime.Now - payEndDate).Days;
                if (delayDays > 0)
                {
                    double rate = 0;
                    var interestRate = _optionService.GetRates().FirstOrDefault();
                    if (interestRate != null)
                        rate = interestRate.Rate;

                    // 利息－依繳納當日郵政儲金匯業局一年期定期存款固定利率按日加計
                    interest = Math.Round(currentPrice * rate / 100 / 365 * delayDays, 0, MidpointRounding.AwayFromZero);
                    // 滯納金－每逾一日按滯納之金額加徵百分之○．五滯納金
                    delayPrice = Math.Round(currentPrice * 0.005 * delayDays, 0, MidpointRounding.AwayFromZero);
                }

                double sumPrice = currentPrice + interest + delayPrice;
                ABUDF_1 abudf_1 = _accessService.GetABUDF_1(form);          
                string transNo = ((abudf_1?.FLNO?.Length == 16) ? abudf_1?.FLNO?.Substring(10, 6) : "000000");
                if (abudf_1 == null)
                {
                    abudf_1 = new ABUDF_1();                   
                    abudf_1.C_NO = form.C_NO;
                    abudf_1.SER_NO = form.SER_NO;
                    abudf_1.P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "1" : "2";
                    abudf_1.P_DATE = verifyDate.AddYears(-1911).ToString("yyyMMdd");
                    abudf_1.E_DATE = payEndDate.AddYears(-1911).ToString("yyyMMdd");

                    // 取得聯單序號
                    if (transNo.Length < 16 || !transNo.StartsWith(base.botCode))
                        transNo = _accessService.GetFLNo(verifyDate.AddYears(-1911).ToString("yyyMMdd"));

                    abudf_1.FLNO = BotHelper.GetPayNo(transNo, sumPrice.ToString(), abudf_1.E_DATE);
                    abudf_1.F_AMT = sumPrice;
                    abudf_1.B_AMT = 0;
                    abudf_1.KEYIN = "EPB02";
                    abudf_1.C_DATE = DateTime.Now;
                    abudf_1.M_DATE = DateTime.Now;
                    // 寫入 ABUDF_1
                    _accessService.AddABUDF_1(abudf_1);
                }
                

                string barcodeMarketA = BotHelper.GetMarketNo(abudf_1.E_DATE);
                string barcodeMarketB = abudf_1.FLNO;
                string barcodeMarketC = BotHelper.GetMarketAmt("0032", sumPrice.ToString(), abudf_1.FLNO, abudf_1.E_DATE);
                string barcodePostA = "19834251";
                string barcodePostB = BotHelper.GetPostNo(transNo, abudf_1.F_AMT.ToString(), abudf_1.E_DATE);
                string barcodePostC = BotHelper.GetPostAmt(abudf_1.F_AMT.ToString());

                var wb = new XLWorkbook(templateFile);
                var ws = wb.Worksheet(1);
                ws.Cell("B2").SetValue(ws.Cell("B2").GetText().Replace("#VerifyDate#", verifyDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("M2").SetValue(ws.Cell("M2").GetText().Replace("#VerifyDate#", verifyDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("O3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("D4").SetValue(form.COMP_NAM);
                ws.Cell("O4").SetValue(form.S_NAME);
                ws.Cell("D5").SetValue(form.S_NAME);
                ws.Cell("O5").SetValue(form.COMP_NAM);
                ws.Cell("F6").SetValue(form.B_SERNO);
                ws.Cell("D7").SetValue(form.P_KIND);
                ws.Cell("F7").SetValue(ws.Cell("F7").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("O7").SetValue(ws.Cell("O7").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("D8").SetValue(ws.Cell("D8").GetText().Replace("#PayEndDate#", payEndDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("O8").SetValue(totalPrice.ToString("N0"));
                ws.Cell("D9").SetValue(currentPrice.ToString("N0"));
                ws.Cell("O9").SetValue(this.GetChineseMoney(totalPrice.ToString()));
                ws.Cell("D10").SetValue(delayPrice.ToString("N0"));
                ws.Cell("D11").SetValue(interest.ToString("N0"));
                ws.Cell("D12").SetValue(sumPrice.ToString("N0"));
                ws.Cell("M12").SetValue(form.B_SERNO);
                ws.Cell("D13").SetValue(this.GetChineseMoney(sumPrice.ToString()));
                ws.Cell("B17").SetValue(ws.Cell("B17").GetText().Replace("#VerifyDate#", verifyDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D18").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("I18").SetValue(form.COMP_NAM);
                ws.Cell("D19").SetValue(form.S_NAME);
                ws.Cell("P19").SetValue(form.B_SERNO);
                ws.Cell("D20").SetValue(form.P_KIND);
                ws.Cell("F20").SetValue(ws.Cell("F20").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("D21").SetValue(ws.Cell("D21").GetText().Replace("#PayEndDate#", payEndDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D22").SetValue(currentPrice.ToString("N0"));
                ws.Cell("D23").SetValue(delayPrice.ToString("N0"));
                ws.Cell("I23").SetValue(interest.ToString("N0"));
                ws.Cell("D24").SetValue(sumPrice.ToString("N0"));
                ws.Cell("D25").SetValue(this.GetChineseMoney(sumPrice.ToString()));
                ws.Cell("B29").SetValue(ws.Cell("B29").GetText().Replace("#VerifyDate#", verifyDate.AddYears(-1911).ToString("yyy年MM月dd日")));
                ws.Cell("D30").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("I30").SetValue(form.COMP_NAM);
                ws.Cell("D31").SetValue(form.S_NAME);
                ws.Cell("P31").SetValue(form.B_SERNO);
                ws.Cell("D32").SetValue(form.P_KIND);
                ws.Cell("F32").SetValue(ws.Cell("F32").GetText().Replace("#P_NUM#", form.P_KIND == "一次全繳" ? "1" : "2").Replace("#P_TIME#", abudf_1.P_TIME));
                ws.Cell("O32").SetValue(payEndDate.AddYears(-1911).ToString("yyy年MM月dd日"));
                ws.Cell("D34").SetValue(currentPrice.ToString("N0"));
                ws.Cell("I34").SetValue(delayPrice.ToString("N0"));
                ws.Cell("O34").SetValue(interest.ToString("N0"));
                ws.Cell("D35").SetValue(sumPrice.ToString("N0"));
                ws.Cell("G35").SetValue(ws.Cell("G35").GetText().Replace("#F_AMTC#", this.GetChineseMoney(sumPrice.ToString())));
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
                Logger.Error($"CreatePaymentPDF: {ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生結清證明
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateProofPDF(FormView form)
        {
            try
            {
                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\結清證明.xlsx";
                // 如果已經產生過檔案，直接下載
                string existFile = $@"{_paymentPath}\Download\{form.C_NO}-{form.SER_NO}結清證明.pdf";
                //if (File.Exists(existFile))
                //    return existFile;

                var wb = new XLWorkbook(templateFile);
                var ws = wb.Worksheet(1);
                ws.Cell("B2").SetValue(form.COMP_NAM);
                ws.Cell("B3").SetValue($"{form.C_NO}-{form.SER_NO}");
                ws.Cell("C4").SetValue(form.ADDR);
                ws.Cell("C5").SetValue(form.B_SERNO);
                ws.Cell("C6").SetValue(form.S_NAME);
                ws.Cell("E28").SetValue(DateTime.Now.AddYears(-1911).ToString("yyy"));
                ws.Cell("I28").SetValue(DateTime.Now.ToString("MM"));
                ws.Cell("N28").SetValue(DateTime.Now.ToString("dd"));

                int idx = 0;
                // 應繳金額
                foreach (char item in form.S_AMT2.ToString().Reverse())
                {
                    ws.Row(9).Cell(16 - idx).SetValue(item.ToString());
                    idx += 2;
                }

                idx = 0;
                // 已繳金額
                double paidAmount = form.S_AMT2.Value - form.P_AMT.Value;
                foreach (char item in paidAmount.ToString().Reverse())
                {
                    ws.Row(10).Cell(16 - idx).SetValue(item.ToString());
                    idx += 2;
                }

                idx = 0;
                // 應退金額
                if (form.CalcStatus == CalcStatus.通過待退費小於4000 || form.CalcStatus == CalcStatus.通過待退費大於4000)
                {
                    double diffMoney = paidAmount - form.S_AMT2.Value;
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
                    double diffMoney = form.S_AMT2.Value - paidAmount;
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
                workbook.Save(existFile);

                return existFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateProofPDF: {ex.Message}");
                throw ex;
            }
        }
    }
}
