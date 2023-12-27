using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class FormService : BaseService
    {
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
                        AND (@S_NAME='' OR S_NAME LIKE '%'+@S_NAME+'%')
                        AND (@S_G_NO='' OR S_G_NO=@S_G_NO)
                        AND (@R_NAME='' OR R_NAME LIKE '%'+@R_NAME+'%')
                        AND (@R_G_NO='' OR R_G_NO=@R_G_NO)
                        AND (@B_SERNO='' OR B_SERNO=@B_SERNO)
                        AND (@R_ADDR3='' OR R_ADDR3 LIKE '%'+@R_ADDR3+'%')
                        AND AP_DATE BETWEEN @StartDate AND @EndDate
                        AND ClientUserID=@ClientUserID",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        S_NAME = filter.S_NAME ?? "",
                        S_G_NO = filter.S_G_NO ?? "",
                        R_NAME = filter.R_NAME ?? "",
                        R_G_NO = filter.R_G_NO ?? "",
                        B_SERNO = filter.B_SERNO ?? "",
                        R_ADDR3 = filter.R_ADDR3 ?? "",
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate.ToString("yyyy-MM-dd 23:59:59"),
                        ClientUserID = BaseService.CurrentUser.ID
                    }).ToList();

                foreach (var item in result)
                {
                    item.Attachments = cn.Query<Attachment>(@"
                        SELECT * FROM Attachment WHERE FormID=@FormID",
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
        public bool AddForm(Form form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(form);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
