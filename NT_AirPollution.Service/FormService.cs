using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace NT_AirPollution.Service
{
    public class FormService : BaseService
    {
        protected readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();

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
                    WHERE (@AutoFormID='' OR AutoFormID=@AutoFormID)
                        AND (@C_NO='' OR C_NO=@C_NO)
                        AND (@CreateUserEmail='' OR CreateUserEmail=@CreateUserEmail)
                        AND (@Status=0 OR Status=@Status)",
                    new
                    {
                        AutoFormID = filter.AutoFormID ?? "",
                        C_NO = filter.C_NO ?? "",
                        CreateUserEmail = filter.CreateUserEmail ?? "",
                        Status = filter.Status
                    }).ToList();

                foreach (var item in forms)
                {
                    item.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment
                        WHERE FormID=@FormID", new { FormID = item.ID });

                    item.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork
                        WHERE FormID=@FormID", new { FormID = item.ID }).ToList();
                }

                return forms;
            }
        }

        /// <summary>
        /// 取得申請單 BY ID
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public Form GetFormByID(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var form = cn.QueryFirstOrDefault<Form>(@"
                    SELECT * FROM Form WHERE ID=@ID",
                    new { ID = id });

                return form;
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
                        AND (@Status=0 OR Status=@Status)
                        AND C_DATE BETWEEN @StartDate AND @EndDate
                        AND ClientUserID=@ClientUserID",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        PUB_COMP = filter.PUB_COMP,
                        COMP_NAM = filter.COMP_NAM ?? "",
                        CreateUserName = filter.CreateUserName ?? "",
                        Status = filter.Status,
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
                }

                return result;
            }
        }

        /// <summary>
        /// 取得表單最新流水號
        /// </summary>
        /// <returns></returns>
        //public int GetSerialNumber()
        //{
        //    using (var cn = new SqlConnection(connStr))
        //    {
        //        int serialNo = cn.QuerySingleOrDefault<int>(@"
        //            SELECT ISNULL(MAX(SerialNo), 0) FROM Form 
        //            WHERE C_DATE>=@Today",
        //            new { Today = DateTime.Now.ToString("yyyy-MM-dd") });

        //        return serialNo;
        //    }
        //}

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

                        // 停復工
                        foreach (var item in form.StopWorks)
                        {
                            item.FormID = id;
                        }
                        cn.Insert(form.StopWorks, trans);

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

                        // 清空停復工
                        cn.Execute(@"DELETE FROM StopWork WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        // 新增停復工
                        cn.Insert(form.StopWorks, trans);

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
        //public int CalcTotalMoney(Form form)
        //{
        //    using (var cn = new SqlConnection(connStr))
        //    {
        //        // 計算施工天數
        //        var diffDays = (form.B_DATE2 - form.E_DATE2).TotalDays + 1;
        //        var projectCodes = cn.GetAll<ProjectCode>().ToList();
        //        var projectCode = projectCodes.First(o => o.ID == form.KIND_NO);
        //        // 基數
        //        double basicNum = 0;
        //        // 費率
        //        double rate = 0;
        //        switch (form.KIND_NO)
        //        {
        //            case "1":
        //            case "2":
        //            case "4":
        //            case "5":
        //            case "6":
        //            case "7":
        //            case "8":
        //            case "9":
        //            case "A":
        //                basicNum = form.AREA_B * diffDays / 30;
        //                break;
        //            case "3":
        //            case "B":
        //                basicNum = form.AREA_B;
        //                break;
        //            case "Z":
        //                basicNum = form.MONEY;
        //                break;
        //        }

        //        if (basicNum >= projectCode.Level1)
        //            rate = projectCode.Rate1;
        //        else if (basicNum * projectCode.Rate3 >= projectCode.Level2)
        //            rate = projectCode.Rate2;
        //        else
        //            rate = projectCode.Rate3;

        //        return Convert.ToInt32(Math.Round(basicNum * rate, 0, MidpointRounding.AwayFromZero));
        //    }
        //}

        public bool SendStatus2(Form form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Status2_{(form.ClientUserID == null ? "NonMember" : "Member")}.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                String content = sr.ReadToEnd();
                // 會員
                string url = string.Format("{0}/Member/Login", _configDomain);
                // 非會員
                if (form.ClientUserID == null)
                    url = string.Format("{0}/Search/Index", _configDomain);

                string body = string.Format(content, form.C_NO, form.ClientUserID == null ? form.CreateUserEmail : form.C_DATE.ToString("yyyy-MM-dd"), url, url, form.FailReason.Replace("\n", "<br>"));

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件需補件通知(案件編號 {form.AutoFormID})",
                            Body = body,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendStatus2: {ex.Message}");
                    throw ex;
                }
            }
        }

        public bool SendStatus3(Form form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Status3.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                String content = sr.ReadToEnd();
                string url1 = string.Format("{0}/Form/Guide", _configDomain);
                string url2 = string.Format("{0}/Search/Index", _configDomain);
                string body = string.Format(content, form.AutoFormID, form.CreateUserEmail, url1, url2);

                // 產生繳款單
                string pdfTemplateFile = $@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Payment.html";
                //var fileBytes = this.GeneratePDF(pdfTemplateFile, form);
                // 儲存實體檔案
                string paymentFile = $@"{_uploadPath}\Payment\繳款單{form.AutoFormID}.pdf";
                using (var fs = new FileStream(paymentFile, FileMode.Create, FileAccess.Write))
                {
                    //fs.Write(fileBytes, 0, fileBytes.Length);
                }

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費通知(案件編號 {form.AutoFormID})",
                            Body = body,
                            Attachment = paymentFile,
                            FailTimes = 0,
                            CreateDate = DateTime.Now
                        });
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"SendStatus3: {ex.Message}");
                    throw ex;
                }
            }
        }

        public bool SendStatus4(Form form)
        {
            string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Status4.txt");
            using (StreamReader sr = new StreamReader(template))
            {
                String content = sr.ReadToEnd();
                string url = string.Format("{0}/Search/Result", _configDomain);
                string body = string.Format(content, form.AutoFormID, form.CreateUserEmail, url);

                // 產生收據
                string pdfTemplateFile = $@"{HostingEnvironment.ApplicationPhysicalPath}\App_Data\Template\Receipt.html";
                //var fileBytes = this.GeneratePDF(pdfTemplateFile, form);
                // 儲存實體檔案
                string receiptFile = $@"{_uploadPath}\Receipt\收據{form.AutoFormID}.pdf";
                using (var fs = new FileStream(receiptFile, FileMode.Create, FileAccess.Write))
                {
                    //fs.Write(fileBytes, 0, fileBytes.Length);
                }

                try
                {
                    using (var cn = new SqlConnection(connStr))
                    {
                        // 寄件夾
                        cn.Insert(new SendBox
                        {
                            Address = form.CreateUserEmail,
                            Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統-案件繳費完成(案件編號 {form.AutoFormID})",
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
                    Logger.Error($"SendStatus4: {ex.Message}");
                    throw ex;
                }
            }
        }
    }
}
