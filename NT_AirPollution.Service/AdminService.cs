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
						AND Password=@Password
                        AND Enabled=1",
                    new
                    {
                        Account = user.Account,
                        Password = user.Password
                    });

                return result;
            }
        }

        /// <summary>
		/// 修改密碼
		/// </summary>
		/// <param name="account">帳號</param>
		/// <param name="password">新密碼</param>
		/// <returns></returns>
		public bool UpdatePassword(string account, string password)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute(@"
						UPDATE dbo.AdminUser
							SET Password=@Password
						WHERE Account=@Account AND Enabled=1",
                    new
                    {
                        Account = account,
                        Password = password
                    });

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 取得全部使用者
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<AdminUser> GetUsers(AdminUserFilterView filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<AdminUser>(@"
					SELECT *
					FROM dbo.AdminUser
					WHERE (@Text='' OR UserName LIKE '%' + @Text + '%' OR Account LIKE '%' + @Text + '%')
						AND (@RoleID IS NULL OR RoleID=@RoleID)
						AND (@Enabled IS NULL OR Enabled=@Enabled)",
                        new
                        {
                            Text = filter.Text,
                            RoleID = filter.RoleID,
                            Enabled = filter.Enabled
                        }).ToList();
                return result;
            }
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(AdminUser user)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(user);
                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 修改使用者
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool UpdateUser(AdminUser user)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(user);
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
