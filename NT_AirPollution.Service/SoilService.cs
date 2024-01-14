using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class SoilService : BaseService
    {
        public SoilPromise GetPromise(AirFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var promise = cn.Query<SoilPromise>(@"
                    SELECT a2.*
                    FROM SoilPromise AS a1
                    INNER JOIN Form AS a2 ON a1.FormID=a2.ID
                    WHERE (@C_NO='' OR a2.C_NO=@C_NO)
                        AND (@COMP_NAM='' OR a2.COMP_NAM LIKE '%'+@COMP_NAM+'%')
                        AND (@TOWN_NO='' OR a2.TOWN_NO=@TOWN_NO)
                        AND (@KIND_NO='' OR a2.KIND_NO=@KIND_NO)
                        AND (@S_NAME='' OR a2.S_NAME LIKE '%'+@S_NAME+'%')
                        AND (@R_NAME='' OR a2.R_NAME LIKE '%'+@R_NAME+'%')
                        AND (@B_DATE='' OR a2.B_DATE>=@B_DATE)
                        AND (@E_DATE='' OR a2.E_DATE<=@E_DATE)",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        COMP_NAM = filter.COMP_NAM ?? "",
                        TOWN_NO = filter.TOWN_NO ?? "",
                        KIND_NO = filter.KIND_NO ?? "",
                        S_NAME = filter.S_NAME ?? "",
                        R_NAME = filter.R_NAME ?? "",
                        B_DATE = filter.StartDate.HasValue ? filter.StartDate.Value.AddYears(-1911).ToString("yyyMMdd") : "",
                        E_DATE = filter.EndDate.HasValue ? filter.EndDate.Value.AddYears(-1911).ToString("yyyMMdd") : "",
                    }).ToList();

                return promise;
            }
        }

        /// <summary>
        /// 取得申報案件的廢土不落地承諾書
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public SoilPromise GetPromiseByFormID(long formID)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var promise = cn.QueryFirstOrDefault<SoilPromise>(@"
                    SELECT a1.*
                    FROM SoilPromise AS a1
                    INNER JOIN Form AS a2 ON a1.FormID=a2.ID
                    WHERE a2.ID=@ID",
                    new { ID = formID });

                return promise;
            }
        }

        /// <summary>
        /// 新增廢土不落地承諾書
        /// </summary>
        /// <param name="promise"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddPromise(SoilPromise promise)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(promise);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"Soil.AddPromise: {ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
