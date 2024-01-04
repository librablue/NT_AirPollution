using Dapper;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class AdminService : BaseService
    {
        /// <summary>
        /// 後台登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public AdminUser Login(AdminUser user)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<AdminUser>(@"
					SELECT * FROM AdminUser 
					WHERE Account=@Account 
						AND Password=@Password",
                    new
                    {
                        Account = user.Account,
                        Password = user.Password
                    });

                return result;
            }
        }
    }
}
