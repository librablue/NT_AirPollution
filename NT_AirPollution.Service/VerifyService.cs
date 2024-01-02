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
    public class VerifyService : BaseService
    {
        /// <summary>
        /// 檢查驗證碼
        /// </summary>
        /// <returns></returns>
        public Form CheckActiveCode(string code)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var form = cn.QueryFirstOrDefault<Form>(@"
                    SELECT * FROM Form 
                    WHERE ActiveCode=@ActiveCode",
                    new { ActiveCode = code });

                return form;
            }
        }

        /// <summary>
        /// 更新驗證狀態
        /// </summary>
        /// <param name="id"></param>
        public void VerifyForm(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                cn.Execute(@"UPDATE Form SET IsActive=1 WHERE ID=@ID",
                    new { ID = id });
            }
        }
    }
}
