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
    public class RoadService : BaseService
    {
        /// <summary>
        /// 取得申報案件的道路認養承諾書
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public RoadPromise GetPromiseByFormID(long formID)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var promise = cn.QueryFirstOrDefault<RoadPromiseView>(@"
                    SELECT a1.*
                        a2.C_NO,a2.SER_NO,a2.COMP_NAM,a2.ADDR,a2.R_M_NAM,a2.R_TEL,a2.R_TEL1
                    FROM RoadPromise AS a1
                    INNER JOIN Form AS a2 ON a1.FormID=a2.ID
                    WHERE a2.ID=@ID",
                    new { ID = formID });

                promise.Roads = cn.Query<Road>(@"
                    SELECT * FROM Road WHERE PromiseID=@PromiseID",
                    new { PromiseID = promise.ID }).ToList();

                return promise;
            }
        }
    }
}
