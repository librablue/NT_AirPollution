﻿using Dapper;
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
    public class OptionService : BaseService
    {
        /// <summary>
        /// 取得鄉鎮清單
        /// </summary>
        /// <returns></returns>
        public List<District> GetDistrict()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.GetAll<District>().ToList();
                return result;
            }
        }

        /// <summary>
        /// 取得工程類別清單
        /// </summary>
        /// <returns></returns>
        public List<ProjectCode> GetProjectCode()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.GetAll<ProjectCode>().ToList();
                return result;
            }
        }

        /// <summary>
        /// 取得附件說明清單
        /// </summary>
        /// <returns></returns>
        public List<AttachmentInfo> GetAttachmentInfo()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.GetAll<AttachmentInfo>().OrderBy(o => o.ID).ToList();
                return result;
            }
        }

        /// <summary>
        /// 取得利率
        /// </summary>
        /// <returns></returns>
        public List<InterestRate> GetRates()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.GetAll<InterestRate>()
                    .OrderByDescending(o => o.Date).ToList();
                return result;
            }
        }

        /// <summary>
        /// 取得所有角色清單
        /// </summary>
        /// <returns></returns>
        public List<AdminRole> GetAdminRoles()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.GetAll<AdminRole>().ToList();
                return result;
            }
        }

        /// <summary>
        /// 新增郵局利率
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddRate(InterestRate rate)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(rate);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddRate: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 刪除郵局利率
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool DeleteRate(InterestRate rate)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Delete(rate);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"DeleteRate: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 取得瀏覽人數
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetVisitor()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var counter = cn.QuerySingle<long>(@"
                        SELECT Counter FROM Visitor");

                return counter;
            }
        }

        /// <summary>
        /// 新增瀏覽人次
        /// </summary>
        /// <returns></returns>
        public bool AddVisitor()
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute(@"UPDATE Visitor SET Counter=Counter+1");
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
