using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
                        Logger.Error($"Road.AddPromise: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 後台查詢報表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<RoadExcel> GetRoadReport(RoadFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<RoadExcel>(@"
                    SELECT DISTINCT a1.PromiseID,a1.RoadID,a1.RoadName,a1.CleanWay1,a1.CleanWay2,
                        a2.StartDate,a2.EndDate,a2.CreateDate AS PromiseCreateDate,
	                    a3.RoadLength,
                        a4.C_NO,a4.SER_NO,a4.COMP_NAM,a4.TOWN_NA,a4.KIND,a4.C_DATE,a4.S_NAME
                    FROM RoadReport AS a1
                    INNER JOIN RoadPromise AS a2 ON a1.PromiseID=a2.ID
                    INNER JOIN Road AS a3 ON a1.RoadID=a3.ID
                    INNER JOIN Form AS a4 ON a2.FormID=a4.ID
                    WHERE (@C_NO=@C_NO OR a4.C_NO=@C_NO)
                        AND (@COMP_NAM=@COMP_NAM OR a4.COMP_NAM=@COMP_NAM)
                        AND (@TOWN_NO='' OR a4.TOWN_NO=@TOWN_NO)
                        AND (@KIND_NO='' OR a4.KIND_NO=@KIND_NO)
                        AND (@S_NAME='' OR a4.S_NAME LIKE '%'+@S_NAME+'%')
                        AND (@R_NAME='' OR a4.R_NAME LIKE '%'+@R_NAME+'%')
                        AND a1.YearMth BETWEEN @StartYear AND @EndYear",
                    new
                    {
                        C_NO = filter.C_NO ?? "",
                        COMP_NAM = filter.COMP_NAM ?? "",
                        TOWN_NO = filter.TOWN_NO ?? "",
                        KIND_NO = filter.KIND_NO ?? "",
                        S_NAME = filter.S_NAME ?? "",
                        R_NAME = filter.R_NAME ?? "",
                        StartYear = $"{filter.Year}00",
                        EndYear = $"{filter.Year}12"
                    }).ToList();

                foreach (var item in result)
                {
                    var roadReport = cn.Query<RoadReport>(@"
                        SELECT * FROM RoadReport
                            WHERE PromiseID=@PromiseID
                                AND RoadID=@RoadID
                                AND YearMth BETWEEN @StartMth AND @EndMth",
                        new
                        {
                            PromiseID = item.PromiseID,
                            RoadID = item.RoadID,
                            StartMth = $"{filter.Year}01",
                            EndMth = $"{filter.Year}12"
                        }).OrderBy(o => o.YearMth).ToList();

                    foreach (var sub in roadReport)
                    {
                        double totalLength = sub.TotalLength / 1000;
                        item.RoadExcelMonth.Add(new RoadExcelMonth
                        {
                            Month = Convert.ToInt16(sub.YearMth.ToString().Substring(4, 2)),
                            CleanLength1 = sub.CleanWay1 == "洗街" ? Math.Round(totalLength, 1, MidpointRounding.AwayFromZero) : 0,
                            CleanLength2 = sub.CleanWay1 == "掃街" ? Math.Round(totalLength, 1, MidpointRounding.AwayFromZero) : 0
                        });
                    }
                }

                return result;
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
                    SELECT a1.* 
                    FROM RoadReport AS a1
                    INNER JOIN RoadPromise AS a2 ON a1.PromiseID=a2.ID
                    WHERE a2.FormID=@FormID
                    ORDER BY a1.YearMth DESC,a1.RoadID",
                    new { FormID = formID }).ToList();
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
