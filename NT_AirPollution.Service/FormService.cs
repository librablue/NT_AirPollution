using Aspose.Cells;
using AutoMapper;
using ClosedXML.Excel;
using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace NT_AirPollution.Service
{
    public class FormService : BaseService
    {
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
                        AND (@StartDate IS NULL OR @EndDate IS NULL OR AP_DATE BETWEEN @StartDate AND @EndDate)
                        AND (@FormStatus=-1 OR FormStatus=@FormStatus)
                        AND (@CalcStatus=-1 OR CalcStatus=@CalcStatus)
                        AND (@VerifyStage1=-1 OR VerifyStage1=@VerifyStage1)
                        AND (@VerifyStage2=-1 OR VerifyStage2=@VerifyStage2)",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        StartDate = filter.StartDate?.AddYears(-1911).ToString("yyyMMdd"),
                        EndDate = filter.EndDate?.AddYears(-1911).ToString("yyyMMdd"),
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
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    item.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    item.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
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
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    result.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = result.ID });

                    result.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
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
                        AND C_DATE BETWEEN @StartDate AND @EndDate
                        AND ClientUserID=@ClientUserID",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        PUB_COMP = filter.PUB_COMP,
                        COMP_NAM = filter.COMP_NAM ?? "",
                        CreateUserName = filter.CreateUserName ?? "",
                        StartDate = filter.StartDate.Value,
                        EndDate = filter.EndDate.Value.ToString("yyyy-MM-dd 23:59:59"),
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
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    item.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    if (item.FormB == null)
                    {
                        double workDays = (item.E_DATE.ToWestDate() - item.B_DATE.ToWestDate()).TotalDays + 1;
                        double downDays = item.StopWorks.Sum(o => o.DOWN_DAY);

                        string B_STAT;
                        if (item.P_KIND == "一次全繳")
                            B_STAT = "A一次繳清無結算";
                        else
                            B_STAT = "B分期繳交待結算";

                        if (item.S_AMT <= 100)
                            B_STAT = "Z已申報結算";

                        item.FormB = new FormB
                        {
                            FormID = item.ID,
                            C_NO = item.C_NO,
                            SER_NO = item.SER_NO,
                            AP_DATE1 = null,
                            B_STAT = B_STAT,
                            B_CSTAT = "",
                            KIND_NO = item.KIND_NO,
                            KIND = item.KIND,
                            YEAR = item.YEAR,
                            A_KIND = item.A_KIND,
                            MONEY = item.MONEY,
                            AREA = item.AREA,
                            AREA2 = item.AREA2,
                            VOLUMEL = item.VOLUMEL,
                            RATIOLB = item.RATIOLB,
                            DENSITYL = item.DENSITYL,
                            B_DATE = item.B_DATE,
                            E_DATE = item.E_DATE,
                            B_YEAR = Math.Round((workDays - downDays) / 365, 2, MidpointRounding.AwayFromZero),
                            S_AMT = item.S_AMT2,
                            T_DAY = workDays - downDays,
                            AREA_B = item.AREA_B,
                            AREA_F = item.AREA_F,
                            PERC_B = item.PERC_B,
                            PRE_C_AMT = item.S_AMT > item.S_AMT2 ? item.S_AMT - item.S_AMT2 : 0,
                            PRE_C_AMT1 = item.S_AMT2 > item.S_AMT ? item.S_AMT2 - item.S_AMT : 0,
                            B_KIND1 = "無",
                            B_KIND2 = "無",
                            ID_DOC1 = "無",
                            ID_DOC2 = "無",
                            ID_DOC3 = "無",
                            COMP_DOC1 = "無",
                            COMP_DOC2 = "無",
                            COMP_DOC3 = "無",
                            BUD_DOC1 = "無",
                            BUD_DOC2 = "無",
                            BUD_DOC3 = "無",
                            WRONG_AP = "否"
                        };
                    }

                    item.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
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
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    item.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    item.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();


                    // 檢查今天是否在停復工日期範圍內
                    bool isPause = result.Any(o => o.StopWorks.Any(x => x.DOWN_DATE2 < now && now > x.UP_DATE2));

                    if (isPause)
                        item.WorkStatus = WorkStatus.停工中;
                    else if (now > item.E_DATE.ToWestDate())
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
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    item.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    item.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();
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
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    item.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    item.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
                        new { FormID = item.ID }).ToList();
                }

                return result;
            }
        }

        /// <summary>
        /// 產生繳費單排程使用
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FormView> GetFormByAccessWorker()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var forms = cn.Query<FormView>(@"
                    SELECT * FROM Form
                    WHERE (VerifyStage1=3 AND FormStatus=3)
                        OR (VerifyStage2=3 AND CalcStatus=3)");

                foreach (var form in forms)
                {
                    form.RefundBank = cn.QueryFirstOrDefault<RefundBank>(@"
                        SELECT * FROM RefundBank WHERE FormID=@FormID",
                        new { FormID = form.ID });
                    if (form.RefundBank == null) form.RefundBank = new RefundBank();

                    form.Payments = cn.Query<Payment>(@"
                        SELECT * FROM Payment WHERE FormID=@FormID",
                        new { FormID = form.ID }).ToList();

                    form.StopWorks = cn.Query<StopWork>(@"
                        SELECT * FROM StopWork WHERE FormID=@FormID",
                        new { FormID = form.ID }).ToList();

                    foreach (var sub in form.StopWorks)
                    {
                        sub.DOWN_DATE2 = sub.DOWN_DATE.ToWestDate();
                        sub.UP_DATE2 = sub.UP_DATE.ToWestDate();
                    }

                    form.FormB = cn.QueryFirstOrDefault<FormB>(@"
                        SELECT * FROM FormB WHERE FormID=@FormID",
                        new { FormID = form.ID });

                    form.FormSub = cn.Query<FormSub>(@"
                        SELECT * FROM FormSub WHERE FormID=@FormID",
                        new { FormID = form.ID }).ToList();
                }

                return forms;
            }
        }

        /// <summary>
        /// 取得申請表單數
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetFormsCount(DateTime? startDate = null, DateTime? endDate = null)
        {
            // 將 DateTime 轉為 7 碼民國年字串 (例: 2026-01-01 -> "1150101")
            string startTaiwanDate = startDate.HasValue
                ? $"{startDate.Value.AddYears(-1911).ToString("yyyMMdd")}"
                : null;

            string endTaiwanDate = endDate.HasValue
                ? $"{endDate.Value.AddYears(-1911).ToString("yyyMMdd")}"
                : null;

            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QuerySingle<long>(@"
                    SELECT COUNT(*) FROM Form
                    WHERE (@startDate IS NULL OR AP_DATE >= @startDate)
                        AND (@endDate IS NULL OR AP_DATE <= @endDate)",
                    new { startDate = startTaiwanDate, endDate = endTaiwanDate });

                return result;
            }
        }

        /// <summary>
        /// 取得繳費人數
        /// </summary>
        /// <returns></returns>
        public long GetPaymentCount(DateTime? startDate = null, DateTime? endDate = null)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QuerySingle<long>(@"
                    SELECT COUNT(*) FROM Payment
                    WHERE BankLog IS NOT NULL
                      AND (@startDate IS NULL OR PayDate >= @startDate)
                      AND (@endDate IS NULL OR PayDate <= @endDate)", new { startDate, endDate });

                return result;
            }
        }

        /// <summary>
        /// 取得減碳量
        /// </summary>
        /// <returns></returns>
        public double GetCarbon(DateTime? startDate = null, DateTime? endDate = null)
        {
            // 將 DateTime 轉為 7 碼民國年字串 (例: 2026-01-01 -> "1150101")
            string startTaiwanDate = startDate.HasValue
                ? $"{startDate.Value.AddYears(-1911).ToString("yyyMMdd")}"
                : null;

            string endTaiwanDate = endDate.HasValue
                ? $"{endDate.Value.AddYears(-1911).ToString("yyyMMdd")}"
                : null;

            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QuerySingle<double>(@"
                    SELECT ISNULL(SUM(
                        a2.Carbon * (
                            (CASE WHEN a1.AP_DATE IS NOT NULL 
                                   AND (@startDate IS NULL OR a1.AP_DATE >= @startDate) 
                                   AND (@endDate IS NULL OR a1.AP_DATE <= @endDate) 
                                  THEN 1 ELSE 0 END)
                            +
                            (CASE WHEN a1.AP_DATE1 IS NOT NULL 
                                   AND (@startDate IS NULL OR a1.AP_DATE1 >= @startDate) 
                                   AND (@endDate IS NULL OR a1.AP_DATE1 <= @endDate) 
                                  THEN 1 ELSE 0 END)
                        )
                    ), 0)
                    FROM Form AS a1
                    INNER JOIN District AS a2 ON a1.TOWN_NO = a2.Code",
                    new { startDate = startTaiwanDate, endDate = endTaiwanDate });

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

                        double workDays = (form.E_DATE.ToWestDate() - form.B_DATE.ToWestDate()).TotalDays + 1;
                        double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);

                        string B_STAT;
                        if (form.P_KIND == "一次全繳")
                            B_STAT = "A一次繳清無結算";
                        else
                            B_STAT = "B分期繳交待結算";

                        if (form.S_AMT <= 100)
                            B_STAT = "Z已申報結算";

                        // 寫入 FormB
                        form.FormB = new FormB
                        {
                            FormID = form.ID,
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO,
                            AP_DATE1 = "",
                            B_STAT = B_STAT,
                            B_CSTAT = "",
                            KIND_NO = form.KIND_NO,
                            KIND = form.KIND,
                            YEAR = form.YEAR,
                            A_KIND = form.A_KIND,
                            MONEY = form.MONEY,
                            AREA = form.AREA,
                            VOLUMEL = form.VOLUMEL,
                            RATIOLB = form.RATIOLB,
                            DENSITYL = form.DENSITYL,
                            B_DATE = form.B_DATE,
                            E_DATE = form.E_DATE,
                            B_YEAR = Math.Round((workDays - downDays) / 365, 2, MidpointRounding.AwayFromZero),
                            S_AMT = form.S_AMT2,
                            T_DAY = workDays - downDays,
                            AREA_B = form.AREA_B,
                            AREA_F = form.AREA_F,
                            PERC_B = form.PERC_B,
                            PRE_C_AMT = form.S_AMT > form.S_AMT2 ? form.S_AMT - form.S_AMT2 : 0,
                            PRE_C_AMT1 = form.S_AMT2 > form.S_AMT ? form.S_AMT2 - form.S_AMT : 0,
                            B_KIND1 = "無",
                            B_KIND2 = "無",
                            ID_DOC1 = "無",
                            ID_DOC2 = "無",
                            ID_DOC3 = "無",
                            COMP_DOC1 = "無",
                            COMP_DOC2 = "無",
                            COMP_DOC3 = "無",
                            BUD_DOC1 = "無",
                            BUD_DOC2 = "無",
                            BUD_DOC3 = "無",
                            WRONG_AP = "否"
                        };

                        cn.Insert(form.FormB, trans);

                        // 6.管線開挖工程須多寫入FormSub資料
                        if (form.KIND_NO == "6")
                        {
                            // 寫入 FormSub
                            foreach (var item in form.FormSub)
                            {
                                item.FormID = id;
                            }

                            cn.Insert(form.FormSub, trans);
                        }

                        trans.Commit();
                        return id;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddForm: {ex.StackTrace}|{ex.Message}");
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
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Execute("DELETE FROM dbo.Form WHERE ID=@ID",
                            new { ID = form.ID }, trans);

                        cn.Execute(@"DELETE FROM dbo.FormB WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        cn.Execute(@"DELETE FROM dbo.Payment WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        cn.Execute(@"DELETE FROM dbo.StopWork WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        cn.Execute(@"DELETE FROM dbo.FormSub WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"DeleteForm: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 更新Form單一欄位
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateFormColumn(FormColumnView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute($@"
                        UPDATE dbo.Form
                            SET {form.ColumnName}={form.ColumnValue}
                        WHERE ID={form.FormID}");
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateFormColumn: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 更新結算用的紀錄表
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool AddFormB(FormView form)
        {
            double workDays = (form.FormB.E_DATE.ToWestDate() - form.FormB.B_DATE.ToWestDate()).TotalDays + 1;
            double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);

            string B_STAT;
            if (form.P_KIND == "一次全繳")
                B_STAT = "A一次繳清無結算";
            else
                B_STAT = "B分期繳交待結算";

            if (form.S_AMT <= 100)
                B_STAT = "Z已申報結算";

            var formB = form.FormB;
            formB.FormID = form.ID;
            formB.C_NO = form.C_NO;
            formB.SER_NO = form.SER_NO;
            formB.AP_DATE1 = form.AP_DATE1;
            formB.B_STAT = B_STAT;
            formB.KIND_NO = form.KIND_NO;
            formB.KIND = form.KIND;
            formB.YEAR = form.YEAR;
            formB.A_KIND = form.A_KIND;
            formB.B_YEAR = Math.Round((workDays - downDays) / 365, 2, MidpointRounding.AwayFromZero);
            formB.S_AMT = form.S_AMT2;
            formB.T_DAY = workDays - downDays;
            formB.PRE_C_AMT = form.S_AMT > form.S_AMT2 ? form.S_AMT - form.S_AMT2 : 0;
            formB.PRE_C_AMT1 = form.S_AMT2 > form.S_AMT ? form.S_AMT2 - form.S_AMT : 0;
            formB.KEYIN = "EPB02";
            formB.C_DATE = DateTime.Now;

            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Execute(@"DELETE FROM dbo.FormB WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        cn.Insert(formB, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddFormB: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 新增合併申報工程明細資料
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddFormSub(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Execute(@"DELETE FROM dbo.FormSub WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);

                        // 6.管線開挖工程須多寫入FormSub資料
                        if (form.KIND_NO == "6")
                        {
                            cn.Insert(form.FormSub, trans);
                        }

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddFormSub: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 取得申請單繳費資訊
        /// </summary>
        /// <param name="paymentID"></param>
        /// <param name="payEndDate"></param>
        /// <param name="payAmount"></param>
        /// <returns></returns>
        public Payment GetPayment(string paymentID, DateTime payEndDate, double payAmount)
        {
            using (var cn = new SqlConnection(connStr))
            {
                // 找出銷帳檔的那筆銷帳單號
                var payment = cn.QueryFirstOrDefault<Payment>(@"
                    SELECT * FROM dbo.Payment
                    WHERE (PaymentID=@PaymentID OR PostPaymentID=@PaymentID)
                        AND PayEndDate=@PayEndDate
                        AND (PayableAmount=@PayAmount)",
                    new
                    {
                        PaymentID = paymentID,
                        PayEndDate = payEndDate,
                        PayAmount = payAmount
                    });

                return payment;
            }
        }

        /// <summary>
        /// 取得申請單繳費資訊
        /// </summary>
        /// <param name="paymentID">銷帳編號</param>
        /// <param name="payType">繳費方式</param>
        /// <param name="payEndDate">繳費期限</param>
        /// <param name="payAmount">繳費金額</param>
        /// <returns></returns>
        public Payment GetPayment(string paymentID, string payType, string payEndDate, double payAmount)
        {
            DateTime dtPayEndDate = DateTime.Now;
            string payEndDateCondition = "";
            if(payType != "U" && payType != "C" && payType != "M")
            {
                payEndDateCondition = " AND PayEndDate=@PayEndDate";
                dtPayEndDate = Convert.ToDateTime($"{2011 + Convert.ToInt32(payEndDate.Substring(0, 2))}-{payEndDate.Substring(2, 2)}-{payEndDate.Substring(4, 2)}");
            }

            using (var cn = new SqlConnection(connStr))
            {
                // 找出銷帳檔的那筆銷帳單號
                var payment = cn.QueryFirstOrDefault<Payment>(@"
                    SELECT * FROM dbo.Payment
                    WHERE (PaymentID=@PaymentID OR PostPaymentID=@PaymentID)
                        AND (PayableAmount=@PayAmount)" + payEndDateCondition,
                    new
                    {
                        PaymentID = paymentID,
                        PayAmount = payAmount,
                        PayEndDate = dtPayEndDate.ToString("yyyy-MM-dd 23:59:59")
                    });

                return payment;
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
                try
                {
                    if (payment.ID == 0)
                        cn.Insert(payment);
                    else
                        cn.Update(payment);

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddPayment: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
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
                try
                {
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
                            FormID = payment.FormID
                        });

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdatePayment: {ex.StackTrace}|{ex.Message}");
                    return false;
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
                            item.DOWN_DAY = (item.UP_DATE2 - item.DOWN_DATE2).TotalDays;

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
        /// 數字轉換為中文
        /// </summary>
        /// <param name="inputNum"></param>
        /// <returns></returns>
        private string GetChineseMoney(string inputNum)
        {
            string[] strArr = { "零", "壹", "貳", "參", "肆", "伍", "陸", "柒", "捌", "玖" };
            string[] unitArr = { "元", "拾", "佰", "仟", "萬", "拾", "佰", "仟", "億" };

            char[] tmpArr = inputNum.ToCharArray();
            string result = "";
            int len = tmpArr.Length;

            for (int i = 0; i < len; i++)
            {
                int num = tmpArr[i] - '0'; // 取得當前數字
                int unitIndex = len - 1 - i; // 對應的單位索引

                if (num != 0)
                {
                    result += strArr[num] + unitArr[unitIndex];
                }
                else
                {
                    // 處理「零」的邏輯
                    // 1. 如果不是最後一位，且下一位不是零，則加上「零」
                    // 2. 「萬」與「元」這類大單位通常需要保留（視需求而定，此處以基礎修正為主）
                    if (unitIndex == 4) // 萬位
                    {
                        result += unitArr[unitIndex];
                    }
                    else if (unitIndex == 0) // 個位（元）
                    {
                        if (!result.EndsWith(unitArr[0])) result += unitArr[0];
                    }
                    else if (i < len - 1 && tmpArr[i + 1] != '0')
                    {
                        result += strArr[0];
                    }
                }
            }

            // 最後修飾：處理連續出現「零元」或「零萬」的細節
            result = result.Replace("零萬", "萬").Replace("零元", "元");
            if (result.EndsWith("元")) result += "整";

            return result;
        }

        /// <summary>
        /// 計算總金額(因為前台一開始就把Form資料複製給FormB，所以直接計算FormB即可)
        /// </summary>
        /// <param name="form"></param>
        /// <param name="downDays">停工天數</param>
        /// <returns></returns>
        public CalcMoneyResult CalcTotalMoney(FormView form, double downDays)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var projectCodes = cn.GetAll<ProjectCode>().ToList();

                // 判斷邏輯：若 AP_DATE1 為空則讀取 form，否則讀取 form.FormB
                bool isApDate1Empty = string.IsNullOrEmpty(form.AP_DATE1);

                // 工程類別取值
                string kindNo = isApDate1Empty ? form.KIND_NO : form.FormB.KIND_NO;
                var projectCode = projectCodes.First(o => o.ID == kindNo);

                // 日期取值
                string bDate = isApDate1Empty ? form.B_DATE : form.FormB.B_DATE;
                string eDate = isApDate1Empty ? form.E_DATE : form.FormB.E_DATE;

                var diffDays = ((eDate.ToWestDate() - bDate.ToWestDate()).TotalDays + 1) - downDays;

                double basicNum = 0;

                switch (kindNo)
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
                        double area = isApDate1Empty ? (form.AREA ?? 0) : (form.FormB.AREA ?? 0);
                        basicNum = area * diffDays / 30;
                        break;
                    case "3":
                        basicNum = isApDate1Empty ? (form.AREA2 ?? 0) : (form.FormB.AREA2 ?? 0);
                        break;
                    case "B":
                        basicNum = isApDate1Empty ? (form.VOLUMEL ?? 0) : (form.FormB.VOLUMEL ?? 0);
                        break;
                    case "Z":
                        double money = isApDate1Empty ? form.MONEY : form.FormB.MONEY.Value;
                        double taxMoney = isApDate1Empty ? form.TAX_MONEY : (form.FormB.TAX_MONEY ?? 0);
                        basicNum = money - taxMoney;
                        break;
                }

                // 級數與費率邏輯
                int level = 0;
                string levelStr = "";
                double rate = 0;

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

                return new CalcMoneyResult
                {
                    Level = levelStr,
                    Rate = rate,
                    TotalMoney = Convert.ToDouble(Math.Round(basicNum * rate, 0, MidpointRounding.AwayFromZero))
                };
            }
        }

        /// <summary>
        /// 取得申報計算公式文字
        /// </summary>
        /// <param name="form"></param>
        /// <param name="downDays"></param>
        /// <returns></returns>
        public string GetApplyFormulaText(FormView form, double downDays)
        {
            using (var cn = new SqlConnection(connStr))
            {
                string formulaText = "";
                var diffDays = (form.E_DATE.ToWestDate() - form.B_DATE.ToWestDate()).TotalDays + 1;
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
                string basicNumFomulaText = "";
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
                        basicNumFomulaText = $"{form.AREA.Value} ╳ {diffDays} / 30";
                        break;
                    case "3":
                        basicNum = form.AREA2.Value;
                        basicNumFomulaText = $"{form.AREA2.Value}";
                        break;
                    case "B":
                        basicNum = form.VOLUMEL.Value;
                        basicNumFomulaText = $"{form.VOLUMEL.Value}";
                        break;
                    case "Z":
                        // 工程合約經費要-營業稅
                        basicNum = form.MONEY - form.TAX_MONEY;
                        basicNumFomulaText = $"{form.MONEY - form.TAX_MONEY}";
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

                formulaText = $"{result.TotalMoney} = {result.Rate} ╳ {basicNumFomulaText}";

                return formulaText;
            }
        }

        /// <summary>
        /// 結算計算公式文字
        /// </summary>
        /// <param name="form"></param>
        /// <param name="downDays"></param>
        /// <returns></returns>
        public string GetCalcFormulaText(FormView form, double downDays)
        {
            using (var cn = new SqlConnection(connStr))
            {
                string formulaText = "";
                var diffDays = ((form.FormB.E_DATE.ToWestDate() - form.FormB.B_DATE.ToWestDate()).TotalDays + 1) - downDays;
                var projectCodes = cn.GetAll<ProjectCode>().ToList();
                var projectCode = projectCodes.First(o => o.ID == form.FormB.KIND_NO);
                // 基數
                double basicNum = 0;
                // 級數
                int level = 0;
                // 級數文字
                string levelStr = "";
                // 費率
                double rate = 0;
                string basicNumFomulaText = "";
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
                        basicNum = form.FormB.AREA.Value * diffDays / 30;
                        basicNumFomulaText = $"{form.FormB.AREA.Value} ╳ {diffDays} / 30";
                        break;
                    case "3":
                        basicNum = form.AREA2.Value;
                        basicNumFomulaText = $"{form.AREA2.Value}";
                        break;
                    case "B":
                        basicNum = form.FormB.VOLUMEL.Value;
                        basicNumFomulaText = $"{form.FormB.VOLUMEL.Value}";
                        break;
                    case "Z":
                        // 工程合約經費要-營業稅
                        basicNum = (form.FormB.MONEY ?? 0) - (form.FormB.TAX_MONEY ?? 0);
                        basicNumFomulaText = $"{(form.FormB.MONEY ?? 0) - (form.FormB.TAX_MONEY ?? 0)}";
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

                formulaText = $"{result.TotalMoney} = {result.Rate} ╳ {basicNumFomulaText}";

                return formulaText;
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\Status2.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\FormStatus3.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\FormStatus4.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\FormStatus{status}.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\Status2.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\CalcStatus3.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\CalcStatus{(int)form.CalcStatus}.txt";
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
            string template = $@"{AppDomain.CurrentDomain.BaseDirectory}App_Data\Template\CalcStatus6.txt";
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
                    Today = DateTime.Now,
                    IsPublic = form.PUB_COMP,
                    StartDate = form.B_DATE.ToWestDate()
                };

                // 申報
                if (string.IsNullOrEmpty(form.AP_DATE1))
                {
                    info.ApplyDate = form.AP_DATE.ToWestDate();
                    info.VerifyDate = form.VerifyDate1.Value;
                    info.TotalPrice = form.S_AMT.Value;
                    info.CurrentPrice = form.P_AMT.Value;
                }
                // 結算
                else
                {
                    info.ApplyDate = form.AP_DATE1.ToWestDate();
                    info.VerifyDate = form.VerifyDate2.Value;
                    info.TotalPrice = form.S_AMT2.Value;

                    // 如果申報金額小於100免繳費，結算金額大於100則以結算金額為繳費金額，否則以結算金額-申報金額為繳費金額
                    if (form.P_AMT.Value <= 100 && form.S_AMT2.Value > 100)
                    {
                        info.CurrentPrice = form.S_AMT2.Value;
                    }
                    else
                    {
                        info.CurrentPrice = form.S_AMT2.Value - form.P_AMT.Value;
                    }
                }

                // 計算繳費資訊(回傳原物件)
                var res = CalcPayment(info);
                // 結算沒有滯納金&利息，繳費期限為結算日+60天
                if (!string.IsNullOrEmpty(form.AP_DATE1))
                {
                    res.Interest = 0;
                    res.Penalty = 0;
                    res.PayEndDate = info.ApplyDate.AddDays(60);
                    // 如果超過60天就以今天為繳費期限
                    if (DateTime.Now.Date > res.PayEndDate.Date)
                    {
                        res.PayEndDate = DateTime.Now;
                    }
                }

                double sumPrice = Math.Round(res.CurrentPrice + res.Interest + res.Penalty, 0);
                ABUDF_1 abudf_1InDB = _accessService.GetABUDF_1(form);
                string transNo = ((abudf_1InDB?.FLNO?.Length == 16) ? abudf_1InDB?.FLNO?.Substring(10, 6) : "000000");

                // 填發日期
                DateTime pdate;

                #region 寫入ABUDF_1
                ABUDF_1 abudf_1 = new ABUDF_1();
                abudf_1.C_NO = form.C_NO;
                abudf_1.SER_NO = form.SER_NO;
                abudf_1.P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "01" : "02";

                // 結算的填發日要用審核結算通過的那天
                if (string.IsNullOrEmpty(form.AP_DATE1))
                {
                    pdate = form.AP_DATE.ToWestDate();
                    abudf_1.P_DATE = pdate.AddYears(-1911).ToString("yyyMMdd");
                }
                else
                {
                    pdate = form.AP_DATE1.ToWestDate();
                    abudf_1.P_DATE = form.FIN_DATE;
                }

                // 退費不用填繳費期限
                abudf_1.E_DATE = sumPrice > 0 ? res.PayEndDate.AddYears(-1911).ToString("yyyMMdd") : null;

                // transNo為預設值或超過繳費期限，要重新產生銷帳單號
                if (transNo == "000000" || DateTime.Now.Date > res.PayEndDate.Date)
                {
                    // 產生新的聯單序號
                    transNo = _accessService.GetFLNo(pdate.AddYears(-1911).ToString("yyyMMdd"));
                }

                abudf_1.FLNO = BotHelper.GetPayNo(transNo, sumPrice.ToString(), abudf_1.E_DATE);
                abudf_1.F_AMT = sumPrice > 0 ? sumPrice : 0;
                abudf_1.B_AMT = sumPrice > 0 ? 0 : Math.Abs(sumPrice);
                abudf_1.KEYIN = "EPB02";
                abudf_1.C_DATE = DateTime.Now;
                abudf_1.M_DATE = DateTime.Now;
                // 寫入 ABUDF_1
                _accessService.AddABUDF_1(abudf_1);
                #endregion


                // 產生條碼
                string barcodeMarketA = BotHelper.GetMarketNo(abudf_1.E_DATE);
                string barcodeMarketB = abudf_1.FLNO;
                string barcodeMarketC = BotHelper.GetMarketAmt("0032", sumPrice.ToString(), abudf_1.FLNO, abudf_1.E_DATE);
                string barcodePostA = "19834251";
                string barcodePostB = BotHelper.GetPostNo(transNo, sumPrice.ToString(), abudf_1.E_DATE);
                string barcodePostC = BotHelper.GetPostAmt(sumPrice.ToString());


                #region 寫入Payment
                var payment = this.GetPayment(abudf_1.FLNO, res.PayEndDate, sumPrice);
                if (payment == null)
                {
                    payment = new Payment
                    {
                        ID = 0,
                        FormID = form.ID,
                        Term = abudf_1.P_TIME,
                        PayEndDate = res.PayEndDate,
                        PaymentID = barcodeMarketB,
                        PostPaymentID = barcodePostB,
                        PayableAmount = sumPrice,
                        Penalty = res.Penalty,
                        Interest = res.Interest,
                        Percent = res.Rate,
                        CreateDate = DateTime.Now
                    };
                }
                else
                {
                    payment.FormID = form.ID;
                    payment.Term = abudf_1.P_TIME;
                    payment.PayEndDate = res.PayEndDate;
                    payment.PayableAmount = sumPrice;
                    payment.Penalty = res.Penalty;
                    payment.Interest = res.Interest;
                    payment.Percent = res.Rate;
                    payment.CreateDate = DateTime.Now;
                }

                this.AddPayment(payment);
                #endregion


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
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");
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
                string templateFile = $@"{_paymentPath}\Template\結清證明.docx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{form.C_NO}-{form.SER_NO}結清證明.pdf";

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");

                Aspose.Words.Document doc = new Aspose.Words.Document(templateFile);


                doc.Range.Replace("{COMP_NAM}", form.COMP_NAM ?? "");
                doc.Range.Replace("{C_NO}", $"{form.C_NO}-{form.SER_NO}");
                doc.Range.Replace("{ADDR}", form.ADDR ?? "");
                doc.Range.Replace("{B_SERNO}", form.B_SERNO ?? "");
                doc.Range.Replace("{S_NAME}", form.S_NAME ?? "");
                doc.Range.Replace("{S_AMT}", form.S_AMT.GetValueOrDefault().ToString("N0"));
                doc.Range.Replace("{S_AMT2}", form.S_AMT2.GetValueOrDefault().ToString("N0"));

                if (form.S_AMT2.GetValueOrDefault() > form.S_AMT.GetValueOrDefault())
                {
                    doc.Range.Replace("{DiffStr}", "結算應補繳交空污費");
                }
                else
                {
                    doc.Range.Replace("{DiffStr}", "結算應退已缴空污費");
                }

                doc.Range.Replace("{DiffAmt}", Math.Abs(form.S_AMT.GetValueOrDefault() - form.S_AMT2.GetValueOrDefault()).ToString("N0"));

                var payment = form.Payments.FirstOrDefault(o => o.Term == "01");
                doc.Range.Replace("{Penalty}", (payment.Penalty.GetValueOrDefault() + payment.Interest.GetValueOrDefault()).ToString("N0"));

                doc.Range.Replace("{Year}", DateTime.Now.AddYears(-1911).ToString("yyy"));
                doc.Range.Replace("{Month}", DateTime.Now.ToString("MM"));
                doc.Range.Replace("{Date}", DateTime.Now.ToString("dd"));

                doc.Save(resultFile);

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
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");
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
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");

                Aspose.Words.Document doc = new Aspose.Words.Document(templateFile);
                doc.Range.Replace("{S_NAME}", form.S_NAME ?? "");
                doc.Range.Replace("{COMP_NAM}", form.COMP_NAM ?? "");
                doc.Range.Replace("{C_NO}", $"{form.C_NO}-{form.SER_NO}");
                doc.Range.Replace("{B_SERNO}", form.B_SERNO ?? "");
                doc.Range.Replace("{P_AMT}", form.P_AMT.Value.ToString("N0"));
                var payment = form.Payments.FirstOrDefault(o => o.Term == "01");
                if (payment?.Penalty > 0)
                    doc.Range.Replace("{Penalty}", $"(不含滯納金{payment.Penalty}元)");
                else
                    doc.Range.Replace("{Penalty}", "");

                doc.Range.Replace("{A_DATE}", payment?.PayDate?.ToString("yyyy-MM-dd") ?? "");
                doc.Range.Replace("{Year}", DateTime.Now.AddYears(-1911).ToString("yyy"));
                doc.Range.Replace("{Month}", DateTime.Now.AddYears(-1911).ToString("MM"));
                doc.Range.Replace("{Date}", DateTime.Now.AddYears(-1911).ToString("dd"));
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
        /// 產生首期申報表
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateFormPDF1(FormView form)
        {
            try
            {
                ChineseMoneyConverter converter = new ChineseMoneyConverter();

                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\首期申報表.docx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{(string.IsNullOrEmpty(form.C_NO) ? "" : $"{form.C_NO}-{form.SER_NO}")}首期申報表.pdf";

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");

                Aspose.Words.Document doc = new Aspose.Words.Document(templateFile);
                doc.Range.Replace("{COMP_NAM}", form.COMP_NAM ?? "");
                doc.Range.Replace("{C_NO}", string.IsNullOrEmpty(form.C_NO) ? "" : $"{form.C_NO}-{form.SER_NO}");
                doc.Range.Replace("{ADDR}", form.ADDR ?? "");
                doc.Range.Replace("{B_SERNO}", form.B_SERNO ?? "");
                doc.Range.Replace("{KIND_NO}", form.KIND_NO ?? "");
                doc.Range.Replace("{STATE}", form.STATE ?? "");
                doc.Range.Replace("{S_NAME}", form.S_NAME ?? "");
                doc.Range.Replace("{S_G_NO}", form.S_G_NO ?? "");
                doc.Range.Replace("{S_ADDR1}", form.S_ADDR1 ?? "");
                doc.Range.Replace("{S_ADDR2}", form.S_ADDR2 ?? "");
                doc.Range.Replace("{S_TEL}", form.S_TEL ?? "");
                doc.Range.Replace("{S_B_NAM}", form.S_B_NAM ?? "");
                doc.Range.Replace("{S_B_TIT}", form.S_B_TIT ?? "");
                doc.Range.Replace("{S_B_ID}", form.S_B_ID ?? "");
                doc.Range.Replace("{S_C_NAM}", form.S_C_NAM ?? "");
                doc.Range.Replace("{S_C_TIT}", form.S_C_TIT ?? "");
                doc.Range.Replace("{S_C_ID}", form.S_C_ID ?? "");
                doc.Range.Replace("{S_C_ADDR}", form.S_C_ADDR ?? "");
                doc.Range.Replace("{S_C_TEL}", form.S_C_TEL ?? "");
                doc.Range.Replace("{R_NAME}", form.R_NAME ?? "");
                doc.Range.Replace("{R_G_NO}", form.R_G_NO ?? "");
                doc.Range.Replace("{R_ADDR1}", form.R_ADDR1 ?? "");
                doc.Range.Replace("{R_ADDR2}", form.R_ADDR2 ?? "");
                doc.Range.Replace("{R_TEL}", form.R_TEL ?? "");
                doc.Range.Replace("{R_B_NAM}", form.R_B_NAM ?? "");
                doc.Range.Replace("{R_B_TIT}", form.R_B_TIT ?? "");
                doc.Range.Replace("{R_B_ID}", form.R_B_ID ?? "");
                doc.Range.Replace("{R_ADDR3}", form.R_ADDR3 ?? "");
                doc.Range.Replace("{R_M_NAM}", form.R_M_NAM ?? "");
                doc.Range.Replace("{R_C_NAM}", form.R_C_NAM ?? "");
                doc.Range.Replace("{R_TEL1}", form.R_TEL1 ?? "");
                doc.Range.Replace("{MONEY}", converter.ToChineseUpper(form.MONEY));
                doc.Range.Replace("{C_MONEY}", converter.ToChineseUpper(form.C_MONEY ?? 0));
                doc.Range.Replace("{PERCENT}", form.PERCENT.Value.ToString());

                if (form.KIND_NO == "1" || form.KIND_NO == "2")
                {
                    doc.Range.Replace("{AREA}", form.AREA_B.Value.ToString());
                }
                else if (form.KIND_NO == "3")
                {
                    doc.Range.Replace("{AREA}", form.AREA2.Value.ToString());
                }
                else
                {
                    doc.Range.Replace("{AREA}", form.AREA.Value.ToString());
                }

                doc.Range.Replace("{B_DATE}", $"{form.B_DATE.Substring(0, 3)} 年 {form.B_DATE.Substring(3, 2)} 月 {form.B_DATE.Substring(5, 2)} 日");
                doc.Range.Replace("{E_DATE}", $"{form.E_DATE.Substring(0, 3)} 年 {form.E_DATE.Substring(3, 2)} 月 {form.E_DATE.Substring(5, 2)} 日");
                var sdate = form.B_DATE.ToWestDate();
                var edate = form.E_DATE.ToWestDate();
                var totalDays = (edate - sdate).Days + 1;
                doc.Range.Replace("{TOTAL_DAY}", $"{totalDays}");
                doc.Range.Replace("{P_KIND}", form.P_KIND ?? "");
                doc.Range.Replace("{P_NUM}", form.P_NUM.ToString());
                doc.Range.Replace("{REC_YN}", form.REC_YN ?? "");

                if (form.S_AMT.HasValue)
                {
                    doc.Range.Replace("{S_AMT}", converter.ToChineseUpper(form.S_AMT.Value));
                    doc.Range.Replace("{P_AMT}", converter.ToChineseUpper(form.P_AMT.Value));
                }
                else
                {
                    doc.Range.Replace("{S_AMT}", "");
                    doc.Range.Replace("{P_AMT}", "");
                }


                doc.Save(resultFile);

                return resultFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateFormPDF1: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生結算申報表
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateFormPDF2(FormView form)
        {
            try
            {
                ChineseMoneyConverter converter = new ChineseMoneyConverter();

                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\結算申報表.docx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{(string.IsNullOrEmpty(form.C_NO) ? "" : $"{form.C_NO}-{form.SER_NO}")}結算申報表.pdf";

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");

                Aspose.Words.Document doc = new Aspose.Words.Document(templateFile);
                doc.Range.Replace("{COMP_NAM}", form.COMP_NAM ?? "");
                doc.Range.Replace("{C_NO}", $"{form.C_NO}-{form.SER_NO}");
                doc.Range.Replace("{ADDR}", form.ADDR ?? "");
                doc.Range.Replace("{B_SERNO}", form.B_SERNO ?? "");
                doc.Range.Replace("{S_NAME}", form.S_NAME ?? "");
                doc.Range.Replace("{S_G_NO}", form.S_G_NO ?? "");
                doc.Range.Replace("{S_ADDR1}", form.S_ADDR1 ?? "");
                doc.Range.Replace("{S_ADDR2}", form.S_ADDR2 ?? "");
                doc.Range.Replace("{S_TEL}", form.S_TEL ?? "");
                doc.Range.Replace("{S_B_NAM}", form.S_B_NAM ?? "");
                doc.Range.Replace("{S_B_TIT}", form.S_B_TIT ?? "");
                doc.Range.Replace("{S_B_ID}", form.S_B_ID ?? "");
                doc.Range.Replace("{S_C_NAM}", form.S_C_NAM ?? "");
                doc.Range.Replace("{S_C_TIT}", form.S_C_TIT ?? "");
                doc.Range.Replace("{S_C_ID}", form.S_C_ID ?? "");
                doc.Range.Replace("{S_C_ADDR}", form.S_C_ADDR ?? "");
                doc.Range.Replace("{S_C_TEL}", form.S_C_TEL ?? "");
                doc.Range.Replace("{R_NAME}", form.R_NAME ?? "");
                doc.Range.Replace("{R_G_NO}", form.R_G_NO ?? "");
                doc.Range.Replace("{R_ADDR1}", form.R_ADDR1 ?? "");
                doc.Range.Replace("{R_ADDR2}", form.R_ADDR2 ?? "");
                doc.Range.Replace("{R_TEL}", form.R_TEL ?? "");
                doc.Range.Replace("{R_B_NAM}", form.R_B_NAM ?? "");
                doc.Range.Replace("{R_B_TIT}", form.R_B_TIT ?? "");
                doc.Range.Replace("{R_B_ID}", form.R_B_ID ?? "");
                doc.Range.Replace("{MONEY}", converter.ToChineseUpper(form.MONEY));

                if (form.FormB.KIND_NO == "1" || form.FormB.KIND_NO == "2")
                {
                    doc.Range.Replace("{AREA}", form.FormB.AREA_B.Value.ToString());
                }
                else if (form.FormB.KIND_NO == "3")
                {
                    doc.Range.Replace("{AREA}", form.FormB.AREA2.Value.ToString());
                }
                else
                {
                    doc.Range.Replace("{AREA}", form.FormB.AREA.Value.ToString());
                }

                doc.Range.Replace("{B_DATE}", $"{form.FormB.B_DATE.Substring(0, 3)} 年 {form.FormB.B_DATE.Substring(3, 2)} 月 {form.FormB.B_DATE.Substring(5, 2)} 日");
                doc.Range.Replace("{E_DATE}", $"{form.FormB.E_DATE.Substring(0, 3)} 年 {form.FormB.E_DATE.Substring(3, 2)} 月 {form.FormB.E_DATE.Substring(5, 2)} 日");
                var sdate = form.FormB.B_DATE.ToWestDate();
                var edate = form.FormB.E_DATE.ToWestDate();
                var totalDays = (edate - sdate).Days + 1;
                doc.Range.Replace("{TOTAL_DAY}", $"{totalDays}");
                doc.Range.Replace("{S_AMT}", converter.ToChineseUpper(form.FormB.S_AMT.Value));

                if (form.P_AMT.HasValue)
                    doc.Range.Replace("{P_AMT}", converter.ToChineseUpper(form.P_AMT.Value));
                else
                    doc.Range.Replace("{P_AMT}", "");

                if (form.S_AMT2.HasValue)
                {
                    doc.Range.Replace("{S_AMT2}", converter.ToChineseUpper(form.S_AMT2.Value));
                    doc.Range.Replace("{DiffStr}", form.S_AMT2.GetValueOrDefault() > form.S_AMT.GetValueOrDefault() ? "應繳" : "應退");
                    doc.Range.Replace("{DiffMoney}", converter.ToChineseUpper(Math.Abs(form.S_AMT.GetValueOrDefault() - form.S_AMT2.GetValueOrDefault())));
                }
                else
                {
                    doc.Range.Replace("{S_AMT2}", "");
                    doc.Range.Replace("{DiffStr}", "應繳應退");
                    doc.Range.Replace("{DiffMoney}", "");
                }


                doc.Save(resultFile);

                return resultFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateFormPDF2: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 產生申報證明
        /// </summary>
        /// <param name="form"></param>
        /// <returns>檔案完整路徑</returns>
        public string CreateFormPDF3(FormView form)
        {
            try
            {
                double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays);
                var result = CalcTotalMoney(form, downDays);

                // 範本檔
                string templateFile = $@"{_paymentPath}\Template\申報證明.docx";
                // 結果檔
                string resultFile = $@"{_paymentPath}\Download\{(string.IsNullOrEmpty(form.C_NO) ? "" : $"{form.C_NO}-{form.SER_NO}")}申報證明.pdf";

                Aspose.Words.License license = new Aspose.Words.License();
                license.SetLicense($@"{AppDomain.CurrentDomain.BaseDirectory}/license/Aspose.total.lic");

                Aspose.Words.Document doc = new Aspose.Words.Document(templateFile);
                doc.Range.Replace("{COMP_NAM}", form.COMP_NAM ?? "");
                doc.Range.Replace("{C_NO}", $"{form.C_NO}-{form.SER_NO}");
                doc.Range.Replace("{ADDR}", form.ADDR ?? "");
                doc.Range.Replace("{B_SERNO}", form.B_SERNO ?? "");
                doc.Range.Replace("{S_NAME}", form.S_NAME ?? "");
                doc.Range.Replace("{Level}", result.Level);
                doc.Range.Replace("{MONEY}", $"{form.MONEY.ToString("N0")}");
                doc.Range.Replace("{C_MONEY}", $"{(form.C_MONEY?.ToString("N0") ?? "")}");

                if (form.FormB.KIND_NO == "1" || form.FormB.KIND_NO == "2")
                {
                    doc.Range.Replace("{AREA}", form.FormB.AREA_B.Value.ToString());
                }
                else if (form.FormB.KIND_NO == "3")
                {
                    doc.Range.Replace("{AREA}", form.FormB.AREA2.Value.ToString());
                }
                else
                {
                    doc.Range.Replace("{AREA}", form.FormB.AREA.Value.ToString());
                }

                doc.Save(resultFile);

                return resultFile;
            }
            catch (Exception ex)
            {
                Logger.Error($"CreateFormPDF3: {ex.StackTrace}|{ex.Message}");
                throw ex;
            }
        }

        /// <summary>
        /// 計算繳費相關資訊
        /// 回傳原物件
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

                if (info.Today > result.PayEndDate)
                {
                    // 延遲天數 = 今天 - 開工日
                    result.DelayDays = (info.Today - result.StartDate).Days;
                }
            }
            else
            {
                // 繳費期限 = 當天
                result.PayEndDate = info.Today;
                // 延遲天數 = 今天 - 開工日
                result.DelayDays = (info.Today - result.StartDate).Days;
            }

            if (result.DelayDays <= 30)
            {
                // 滯納金－每逾一日按滯納之金額加徵百分之○．五滯納金
                result.Penalty = Math.Round(result.TotalPrice * 0.005 * result.DelayDays, 0, MidpointRounding.AwayFromZero);
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
                result.Penalty = Math.Round(result.TotalPrice * 0.005 * 30, 0, MidpointRounding.AwayFromZero);

                // 30天後算利息
                // 106/03 前:(應繳金額+滯納金)*郵局儲匯局定存利率(浮動)*(逾期天數-30)/365
                // 106/03 後:(應繳金額)*郵局儲匯局定存利率(浮動)*(逾期天數-30)/365
                if (DateTime.Now < new DateTime(2016, 3, 1))
                {
                    result.Interest = Math.Round((result.TotalPrice + result.Penalty) * result.Rate / 100 * (result.DelayDays - 30) / 365, 0, MidpointRounding.AwayFromZero);
                }
                else
                {
                    result.Interest = Math.Round(result.TotalPrice * result.Rate / 100 * (result.DelayDays - 30) / 365, 0, MidpointRounding.AwayFromZero);
                }
            }

            // 繳費期限當天最後一秒
            result.PayEndDate = result.PayEndDate.Date.AddDays(1).AddSeconds(-1);
            return result;
        }

        /// <summary>
        /// 產生結算退費審核表的說明文字
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public string GenerateRefundComment(FormView form)
        {
            // 溢收總金額與停工天數計算
            double overPayAmount = form.S_AMT.Value > form.S_AMT2.Value ? form.S_AMT.Value - form.S_AMT2.Value : 0;
            double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);

            // 1. 基本參數準備
            string text1 = overPayAmount > 0 ? "核退" : "核補";
            string text2 = Math.Abs(form.S_AMT.Value - form.S_AMT2.Value).ToString("N0");
            string text3 = form.S_AMT2.Value.ToString("N0");
            string text4 = this.GetCalcFormulaText(form, downDays);
            string text5 = form.S_AMT.Value.ToString("N0");
            string text6 = this.GetApplyFormulaText(form, downDays);

            // 2. 計算各項差異 (text7 邏輯)
            double applyWorkDays = (form.E_DATE.ToWestDate() - form.B_DATE.ToWestDate()).TotalDays + 1;
            double calcWorkDays = (form.FormB.E_DATE.ToWestDate() - form.FormB.B_DATE.ToWestDate()).TotalDays + 1;

            double diffDays = (calcWorkDays - downDays) - applyWorkDays; // 工期差
            double diffArea = (form.FormB.AREA ?? 0) - (form.AREA ?? 0);   // 面積差
            double diffMoney = (form.FormB.MONEY ?? 0) - (form.MONEY);     // 經費差
            double diffVOLUMEL = (form.FormB.VOLUMEL ?? 0) - (form.VOLUMEL ?? 0); // 土石外運差

            string statusDays = diffDays > 0 ? $"延長{Math.Abs(diffDays)}天" : (diffDays < 0 ? $"縮短{Math.Abs(diffDays)}天" : "不變");
            string statusArea = diffArea > 0 ? $"增加{Math.Abs(diffArea)}平方公尺" : (diffArea < 0 ? $"減少{Math.Abs(diffArea)}平方公尺" : "不變");
            string statusMoney = diffMoney > 0 ? $"增加{Math.Abs(diffMoney).ToString("N0")}元" : (diffMoney < 0 ? $"減少{Math.Abs(diffMoney).ToString("N0")}元" : "不變");
            string statusVOLUMEL = diffVOLUMEL > 0 ? $"增加{Math.Abs(diffVOLUMEL)}方" : (diffVOLUMEL < 0 ? $"減少{Math.Abs(diffVOLUMEL)}方" : "不變");

            // 3. 組裝 text7
            string text7 = "";
            List<string> unchangedItems = new List<string>();
            List<string> changedItems = new List<string>();

            if (statusDays == "不變") unchangedItems.Add("工期"); else changedItems.Add($"工期{statusDays}");
            if (statusArea == "不變") unchangedItems.Add("面積"); else changedItems.Add($"面積{statusArea}");
            if (statusMoney == "不變") unchangedItems.Add("合約經費"); else changedItems.Add($"合約經費{statusMoney}");
            if (statusVOLUMEL == "不變") unchangedItems.Add("土石外運"); else changedItems.Add($"土石外運{statusVOLUMEL}");


            if (unchangedItems.Count == 4)
            {
                text7 = "工期、面積、合約經費、土石外運不變。";
            }
            else
            {
                string unchangedPart = unchangedItems.Count > 0 ? string.Join("、", unchangedItems) + "不變" : "";
                string changedPart = string.Join("，", changedItems);

                if (!string.IsNullOrEmpty(unchangedPart) && !string.IsNullOrEmpty(changedPart))
                    text7 = $"{changedPart}，{unchangedPart}。";
                else
                    text7 = (changedPart + unchangedPart) + "。";
            }

            // 4. 最終字串組裝
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"一、審核應{text1}： {text2}元");
            sb.AppendLine("");
            sb.AppendLine($"二、結算申報實際應繳金額：{text3} 元");
            sb.AppendLine($"    計算式：{text4}");
            sb.AppendLine("");
            sb.AppendLine($"三、未開工前申報應繳金額：{text5} 元");
            sb.AppendLine($"    計算式：{text6}");
            sb.AppendLine("");
            sb.AppendLine($"四、目前已繳金額(不含逾期利息)：{text5} 元");
            sb.AppendLine("");
            sb.AppendLine("五、減免金額：0 元");
            sb.AppendLine("");
            sb.AppendLine($"六、{text7}");
            sb.AppendLine("");
            sb.Append($"七、應{text1}金額：{text2} 元");

            return sb.ToString();
        }

        /// <summary>
        /// 從Access匯入資料到SQL Server
        /// </summary>
        /// <param name="c_no">管制編號</param>
        /// <param name="bdate">開工日期</param>
        public bool ImportData(string c_no, string bdate)
        {
            ABUDF abudf = _accessService.GetABUDF(c_no, bdate);
            ABUDF_B abudf_b = _accessService.GetABUDF_B(c_no, abudf.SER_NO);
            List<ABUDF_1> abudf_1s = _accessService.GetABUDF_1(c_no, abudf.SER_NO);
            List<ABUDF_I> abudf_is = _accessService.GetABUDF_I(c_no, abudf.SER_NO);

            // 1. 設定 AutoMapper 配置
            var config1 = new MapperConfiguration(cfg => cfg.CreateMap<ABUDF, Form>());
            var mapper1 = config1.CreateMapper();
            var form = mapper1.Map<Form>(abudf);

            var config2 = new MapperConfiguration(cfg => cfg.CreateMap<ABUDF_B, FormB>());
            var mapper2 = config2.CreateMapper();
            var formB = mapper2.Map<FormB>(abudf_b);

            form.ClientUserID = CurrentUser.ID;
            form.CreateUserEmail = CurrentUser.Email;
            form.CreateUserName = CurrentUser.UserName;
            form.LATLNG = string.IsNullOrEmpty(abudf.LATLNG) ? "," : abudf.LATLNG;



            var abudf1 = abudf_1s.FirstOrDefault(o => o.P_TIME == "01");

            /* 申請狀態 */
            // 現場作業是通過才會建資料
            form.VerifyDate1 = abudf.C_DATE;
            form.VerifyStage1 = VerifyStage.複審通過;

            if (abudf.S_AMT > 0 && string.IsNullOrEmpty(abudf.FIN_DATE))
                form.FormStatus = FormStatus.通過待繳費;
            else if (abudf.S_AMT == 0 && string.IsNullOrEmpty(abudf.FIN_DATE))
                form.FormStatus = FormStatus.免繳費;
            else if (abudf1 != null && !string.IsNullOrEmpty(abudf1.F_DATE))
                form.FormStatus = FormStatus.已繳費完成;


            /* 結算狀態 */
            form.CalcStatus = CalcStatus.未申請;

            if (!string.IsNullOrEmpty(abudf_b.AP_DATE1))
            {
                form.CalcStatus = CalcStatus.通過待繳費;
                form.VerifyDate2 = abudf_b.AP_DATE1.ToWestDate();
                form.VerifyStage2 = VerifyStage.複審通過;
            }

            if (!string.IsNullOrEmpty(abudf_b.AP_DATE1) && abudf_b.PRE_C_AMT < 4000)
                form.CalcStatus = CalcStatus.通過待退費小於4000;
            else if (!string.IsNullOrEmpty(abudf_b.AP_DATE1) && abudf_b.PRE_C_AMT >= 4000)
                form.CalcStatus = CalcStatus.通過待退費大於4000;
            else if (!string.IsNullOrEmpty(abudf.FIN_DATE))
                form.CalcStatus = CalcStatus.繳退費完成;


            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long formID = cn.Insert(form);
                    formB.FormID = formID;
                    long formBID = cn.Insert(formB);

                    for (int i = 1; i <= 2; i++)
                    {
                        var abudf_1 = abudf_1s.FirstOrDefault(o => o.P_TIME == $"0{i}");
                        var abudf_i = abudf_is.FirstOrDefault(o => o.P_TIME == $"0{i}");

                        if (abudf_1 == null) continue;

                        Payment payment = new Payment
                        {
                            FormID = formBID,
                            Term = $"{i}",
                            PayEndDate = string.IsNullOrEmpty(abudf_1.E_DATE) ? DateTime.Now : abudf_1.E_DATE.ToWestDate(),
                            PaymentID = abudf_1?.FLNO,
                            PayableAmount = abudf.P_AMT,
                            Penalty = abudf_i?.PEN_AMT,
                            Interest = abudf_i?.I_AMT,
                            Percent = abudf_i?.PERCENT ?? 1.725,
                            PayAmount = abudf_1.F_AMT,
                            PayDate = string.IsNullOrEmpty(abudf_1.PM_DATE) ? (DateTime?)null : abudf_1.PM_DATE.ToWestDate(),
                            CreateDate = abudf_1.C_DATE,
                            ModifyDate = abudf_1.M_DATE
                        };

                        cn.Insert(payment);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"ImportData: {ex.StackTrace}|{ex.Message}");
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 從 Access 匯入資料到 SQL Server
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool SyncData(FormView form)
        {
            ABUDF abudf = _accessService.GetABUDF(form.C_NO, form.SER_NO.Value);
            ABUDF_B abudf_b = _accessService.GetABUDF_B(form.C_NO, form.SER_NO.Value);
            List<ABUDF_DAY> abudf_day = _accessService.GetABUDF_DAY(form.C_NO, form.SER_NO.Value);
            //List<ABUDF_1> abudf_1s = _accessService.GetABUDF_1(form.C_NO, form.SER_NO.Value);
            //List<ABUDF_I> abudf_is = _accessService.GetABUDF_I(form.C_NO, form.SER_NO.Value);

            var config1 = new MapperConfiguration(cfg => { cfg.CreateMap<ABUDF, FormView>(); });
            var mapper1 = config1.CreateMapper();
            mapper1.Map(abudf, form);

            var config2 = new MapperConfiguration(cfg => cfg.CreateMap<ABUDF_B, FormB>());
            var mapper2 = config2.CreateMapper();
            mapper2.Map(abudf_b, form.FormB);

            var config3 = new MapperConfiguration(cfg => cfg.CreateMap<ABUDF_DAY, StopWork>());
            var mapper3 = config3.CreateMapper();
            mapper3.Map(abudf_day, form.StopWorks);

            // 特定欄位邏輯
            form.LATLNG = string.IsNullOrEmpty(abudf.LATLNG) ? "," : abudf.LATLNG;
            form.AP_DATE1 = form.FormB.AP_DATE1;
            form.S_AMT2 = form.FormB.S_AMT;
            form.FormB.FormID = form.ID;
            foreach (var item in form.StopWorks)
            {
                item.FormID = form.ID;
            }

            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        // Form
                        cn.Update(form, trans);

                        // FormB
                        cn.Update(form.FormB, trans);

                        // StopWork
                        cn.Execute(@"DELETE FROM dbo.StopWork WHERE FormID=@FormID",
                            new { FormID = form.ID }, trans);
                        cn.Insert(form.StopWorks, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"SyncData: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }
    }
}
