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
        public List<AttachmentInfoView> GetAttachmentInfo()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<AttachmentInfoView>(@"
                    SELECT a1.*,a2.FileName
                    FROM AttachmentInfo AS a1
                    LEFT JOIN Attachment AS a2 ON a1.ID=a2.InfoID").ToList();
                return result;
            }
        }
    }
}
