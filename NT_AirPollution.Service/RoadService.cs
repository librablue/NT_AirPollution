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
        public RoadPromiseView GetPromiseByFormID(long formID)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var promise = cn.QueryFirstOrDefault<RoadPromiseView>(@"
                    SELECT a1.*
                    FROM RoadPromise AS a1
                    INNER JOIN Form AS a2 ON a1.FormID=a2.ID
                    WHERE a2.ID=@ID",
                    new { ID = formID });

                if (promise != null)
                {
                    promise.Roads = cn.Query<Road>(@"
                    SELECT * FROM Road WHERE PromiseID=@PromiseID",
                    new { PromiseID = promise.ID }).ToList();
                }

                return promise;
            }
        }

        /// <summary>
        /// 新增道路認養承諾書
        /// </summary>
        /// <param name="promise"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long AddPromise(RoadPromiseView promise)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        long id = cn.Insert(promise, trans);

                        // 附件
                        foreach (var item in promise.Roads)
                            item.PromiseID = id;

                        cn.Insert(promise.Roads, trans);

                        trans.Commit();
                        return id;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddPromise: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 取得道路認養成果
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public List<RoadReport> GetReportByFormID(long formID)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var report = cn.Query<RoadReport>(@"
                    SELECT * FROM RoadReport WHERE FormID=@ID
                        ORDER BY YearMth DESC,RoadID",
                    new { ID = formID }).ToList();
                return report;
            }
        }

        /// <summary>
        /// 新增道路認養成果
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public bool AddReport(List<RoadReport> reports)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(reports);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"List: {ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
