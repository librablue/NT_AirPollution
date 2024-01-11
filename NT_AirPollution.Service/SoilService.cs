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
