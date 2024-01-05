using Dapper;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class AccessService : BaseService
    {
        /// <summary>
        /// 取得管制編號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public string GetC_NO(FormView form)
        {
            string chineseYear = form.C_DATE.AddYears(-1911).ToString("yyy");
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault(@"
                    SELECT TOP 1 * FROM ABUDF
                    WHERE 鄉鎮代碼=@TOWN_NO AND 工程類別代碼=@KIND_NO AND 申報日期 BETWEEN @StartDate AND @EndDate
                    ORDER BY 管制編號 DESC",
                    new
                    {
                        TOWN_NO = form.TOWN_NO,
                        KIND_NO = form.KIND_NO,
                        StartDate = $"{chineseYear}0101",
                        EndDate = $"{chineseYear}1231",
                    });

                if (result == null)
                {
                    return $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}001";

                }

                int SerialNo = Convert.ToInt16(result.管制編號.Substring(8, 3));
                return $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}{SerialNo.ToString().PadLeft(3, '0')}";
            }
        }
    }
}
