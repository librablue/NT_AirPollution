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
    public class AirService : BaseService
    {
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
                    SELECT a1.*,
                        a2.C_NO,a2.SER_NO,a2.COMP_NAM,a2.B_DATE,a2.E_DATE 
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
                        Logger.Error($"AddAir: {ex.Message}");
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
                            new { AirID = air.ID}, trans);

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
                        Logger.Error($"UpdateAir: {ex.Message}");
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
                        Logger.Error($"DeleteAir: {ex.Message}");
                        throw new Exception("系統發生未預期錯誤");
                    }
                }
            }
        }
    }
}
