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
    public class DownloadService : BaseService
    {
        /// <summary>
        /// 取得全部下載
        /// </summary>
        /// <returns></returns>
        public List<Downloads> GetDownloads()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var downloads = cn.Query<Downloads>(@"
                    SELECT * FROM dbo.Download 
                    WHERE DeleteDate IS NULL
                    ORDER BY Sort").ToList();

                return downloads;
            }
        }

        /// <summary>
        /// 取得一個下載
        /// </summary>
        /// <param name="id">下載ID</param>
        /// <returns></returns>
        public Downloads GetDownload(int id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var download = cn.Query<Downloads>(@"
                                    SELECT * FROM dbo.Download 
                                    WHERE DeleteDate IS NULL AND Id=@Id",
                                new { Id = id }).FirstOrDefault();
                return download;
            }
        }

        /// <summary>
        /// 新增下載
        /// </summary>
        /// <param name="download"></param>
        public void AddDownload(Download download)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert<Download>(download);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 修改下載
        /// </summary>
        /// <param name="donwload"></param>
        public void UpdateDownload(Download download)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute(@"UPDATE dbo.Download
                                SET Title=@Title,
                                    FileName=@FileName,
                                    Sort=@Sort,
                                    ModifyDate=GETDATE()
                                WHERE Id=@Id",
                                new
                                {
                                    Title = download.Title,
                                    FileName = download.Filename,
                                    Sort = download.Sort,
                                    Id = download.Id
                                });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 刪除下載
        /// </summary>
        /// <param name="id"></param>
        public void DeleteDownload(int id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute(@"UPDATE dbo.Download
                                SET DeleteDate=GETDATE()
                                WHERE Id=@Id",
                                new
                                {
                                    Id = id
                                });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
