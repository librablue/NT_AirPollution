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
    }
}
