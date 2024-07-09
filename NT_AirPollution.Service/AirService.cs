using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NT_AirPollution.Service
{
    public class AirService : BaseService
    { 
        /// <summary>
        /// 後台查詢報表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<AirReport> GetAirs(AirFilter filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var airs = cn.Query<AirReport>(@"
                    SELECT a1.*,a2.*,a1.ID AS AirID
                    FROM Air AS a1
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

                foreach (var item in airs)
                {
                    item.AirFiles = cn.Query<AirFile>(@"
                        SELECT * FROM AirFile WHERE AirID=@AirID",
                        new { AirID = item.AirID }).ToList();
                }

                return airs;
            }
        }

        /// <summary>
        /// 取得申報案件的全部空品不良回報紀錄
        /// </summary>
        /// <param name="formID"></param>
        /// <returns></returns>
        public List<AirView> GetAirsByFormID(long formID)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var airs = cn.Query<AirView>(@"
                    SELECT a1.*
                    FROM Air AS a1
                    INNER JOIN Form AS a2 ON a1.FormID=a2.ID
                    WHERE a2.ID=@ID",
                    new { ID = formID }).ToList();

                foreach (var item in airs)
                {
                    item.AirFiles = cn.Query<AirFile>(@"
                        SELECT * FROM AirFile WHERE AirID=@AirID",
                        new { AirID = item.ID }).ToList();
                }

                return airs;
            }
        }

        /// <summary>
        /// 新增空品不良回報
        /// </summary>
        /// <param name="air"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long AddAir(AirView air)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        long id = cn.Insert(air, trans);

                        // 附件
                        foreach (var item in air.AirFiles)
                            item.AirID = id;

                        cn.Insert(air.AirFiles, trans);

                        trans.Commit();
                        return id;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"AddAir: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 修改空品不良回報
        /// </summary>
        /// <param name="air"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateAir(AirView air)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Update(air, trans);

                        // 清空附件
                        cn.Execute(@"DELETE FROM AirFile WHERE AirID=@AirID",
                            new { AirID = air.ID }, trans);

                        // 附件
                        foreach (var item in air.AirFiles)
                            item.AirID = air.ID;

                        // 新增附件
                        cn.Insert(air.AirFiles, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"UpdateAir: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }

        /// <summary>
        /// 刪除空品不良回報
        /// </summary>
        /// <param name="air"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool DeleteAir(AirView air)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Open();
                using (var trans = cn.BeginTransaction())
                {
                    try
                    {
                        cn.Delete(air, trans);

                        // 清空附件
                        cn.Execute(@"DELETE FROM AirFile WHERE AirID=@AirID",
                            new { AirID = air.ID }, trans);

                        trans.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        Logger.Error($"DeleteAir: {ex.StackTrace}|{ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }
    }
}
