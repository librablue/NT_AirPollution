using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
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
                    item.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                        new { FormID = item.ID });

                    if (!string.IsNullOrEmpty(item.B_DATE))
                        item.B_DATE2 = Convert.ToDateTime($"{Convert.ToInt32(item.B_DATE.Substring(0, 3)) + 1911}-{item.B_DATE.Substring(3, 2)}-{item.B_DATE.Substring(5, 2)}");
                    if (!string.IsNullOrEmpty(item.E_DATE))
                        item.E_DATE2 = Convert.ToDateTime($"{Convert.ToInt32(item.E_DATE.Substring(0, 3)) + 1911}-{item.E_DATE.Substring(3, 2)}-{item.E_DATE.Substring(5, 2)}");
                    if (!string.IsNullOrEmpty(item.S_B_BDATE))
                        item.S_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(item.S_B_BDATE.Substring(0, 3)) + 1911}-{item.S_B_BDATE.Substring(3, 2)}-{item.S_B_BDATE.Substring(5, 2)}");
                    if (!string.IsNullOrEmpty(item.R_B_BDATE))
                        item.R_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(item.R_B_BDATE.Substring(0, 3)) + 1911}-{item.R_B_BDATE.Substring(3, 2)}-{item.R_B_BDATE.Substring(5, 2)}");
                }

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
                        AND AutoFormID=@AutoFormID",
                    new
                    {
                        CreateUserEmail = filter.CreateUserEmail,
                        AutoFormID = filter.AutoFormID
                    });

                result.Attachment = cn.QueryFirstOrDefault<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
                                        new { FormID = result.ID });

                if (!string.IsNullOrEmpty(result.B_DATE))
                    result.B_DATE2 = Convert.ToDateTime($"{Convert.ToInt32(result.B_DATE.Substring(0, 3)) + 1911}-{result.B_DATE.Substring(3, 2)}-{result.B_DATE.Substring(5, 2)}");
                if (!string.IsNullOrEmpty(result.E_DATE))
                    result.E_DATE2 = Convert.ToDateTime($"{Convert.ToInt32(result.E_DATE.Substring(0, 3)) + 1911}-{result.E_DATE.Substring(3, 2)}-{result.E_DATE.Substring(5, 2)}");
                if (!string.IsNullOrEmpty(result.S_B_BDATE))
                    result.S_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(result.S_B_BDATE.Substring(0, 3)) + 1911}-{result.S_B_BDATE.Substring(3, 2)}-{result.S_B_BDATE.Substring(5, 2)}");
                if (!string.IsNullOrEmpty(result.R_B_BDATE))
                    result.R_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(result.R_B_BDATE.Substring(0, 3)) + 1911}-{result.R_B_BDATE.Substring(3, 2)}-{result.R_B_BDATE.Substring(5, 2)}");

                return result;
            }
        }

        /// <summary>
        /// 取得表單最新流水號
        /// </summary>
        /// <returns></returns>
        public int GetSerialNumber()
        {
            using (var cn = new SqlConnection(connStr))
            {
                int serialNo = cn.QuerySingleOrDefault<int>(@"
                    SELECT ISNULL(MAX(SerialNo), 0) FROM Form 
                    WHERE C_DATE>=@Today",
                    new { Today = DateTime.Now.ToString("yyyy-MM-dd") });

                return serialNo;
            }
        }

        /// <summary>
        /// 新增申請單
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool AddForm(FormView form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        long id = cn.Insert(form, trans);
                        form.Attachment.FormID = id;
                        cn.Insert(form.Attachment, trans);
                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error(ex.Message);
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }
    }
}
