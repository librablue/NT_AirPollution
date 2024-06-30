using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class ClientUserService : BaseService
    {
        /// <summary>
        /// 檢查Email帳號是否存在
        /// </summary>
        /// <param name="email">Email</param>
        /// <returns></returns>
        public ClientUser GetExistClientUser(string email)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var user = cn.QueryFirstOrDefault<ClientUser>(@"
					SELECT * FROM dbo.ClientUser WHERE Email=@Email",
                    new { Email = email });

                return user;
            }
        }

        /// <summary>
        /// 查詢使用者 by ID
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ClientUser GetUserByID(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<ClientUser>(@"
                    SELECT * FROM ClientUser
                    WHERE ID=@ID
                        AND DeleteDate IS NULL", new { ID = id });
                return result;
            }
        }

        /// <summary>
        /// 查詢使用者 by email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ClientUser GetUserByEmail(string email)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<ClientUser>(@"
                    SELECT * FROM ClientUser
                    WHERE Email=@Email
                        AND DeleteDate IS NULL", new { Email = email });
                return result;
            }
        }

        /// <summary>
        /// 取得會員人數
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public long GetUserCount()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var counter = cn.QuerySingle<long>(@"
                        SELECT COUNT(*) FROM ClientUser");

                return counter;
            }
        }

        /// <summary>
        /// 新增使用者
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool AddUser(ClientUser user)
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
                    Logger.Error($"AddUser: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 修改使用者
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateClientUser(ClientUser user)
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
                    Logger.Error($"UpdateClientUser: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 前台登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ClientUser Login(ClientUser user)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<ClientUser>(@"
					SELECT * FROM ClientUser 
					WHERE Email=@Email 
						AND Password=@Password
                        AND DeleteDate IS NULL",
                    new
                    {
                        Email = user.Email,
                        Password = user.Password
                    });

                return result;
            }
        }

        /// <summary>
        /// 取得營建業主 ByID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientUserCompany GetCompanyByID(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<ClientUserCompany>(@"
                    SELECT * FROM ClientUserCompany WHERE ID=@ID",
                    new { ID = id });

                if (!string.IsNullOrEmpty(result.S_B_BDATE))
                    result.S_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(result.S_B_BDATE.Substring(0, 3)) + 1911}-{result.S_B_BDATE.Substring(3, 2)}-{result.S_B_BDATE.Substring(5, 2)}");

                return result;
            }
        }

        /// <summary>
        /// 取得使用者所有的營建業主
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public List<ClientUserCompany> GetCompanyByUser(ClientUserCompany filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<ClientUserCompany>(@"
                    SELECT *
                    FROM ClientUserCompany
                    WHERE (@S_G_NO='' OR S_G_NO=@S_G_NO)
                        AND (@S_NAME='' OR S_NAME LIKE '%'+@S_NAME+'%')
                        AND (@S_B_NAM='' OR S_B_NAM LIKE '%'+@S_B_NAM+'%')
                        AND (@S_C_NAM='' OR S_C_NAM LIKE '%'+@S_C_NAM+'%')
                        AND ClientUserID=@ClientUserID",
                        new
                        {
                            S_G_NO = filter.S_G_NO ?? "",
                            S_NAME = filter.S_NAME ?? "",
                            S_B_NAM = filter.S_B_NAM ?? "",
                            S_C_NAM = filter.S_C_NAM ?? "",
                            ClientUserID = filter.ClientUserID
                        }).ToList();

                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.S_B_BDATE))
                        item.S_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(item.S_B_BDATE.Substring(0, 3)) + 1911}-{item.S_B_BDATE.Substring(3, 2)}-{item.S_B_BDATE.Substring(5, 2)}");
                }

                return result;
            }
        }

        /// <summary>
        /// 新增營建業主
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddCompany(ClientUserCompany company)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(company);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddCompany: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 修改營建業主
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateCompany(ClientUserCompany company)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(company);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateCompany: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 刪除營建業主
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool DeleteCompany(ClientUserCompany company)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute(@"
                        DELETE FROM ClientUserCompany
                        WHERE ID=@ID AND ClientUserID=@ClientUserID",
                        new { ID = company.ID, ClientUserID = company.ClientUserID });
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"DeleteCompany: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 取得承包商 ByID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ClientUserContractor GetContractorByID(long id)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<ClientUserContractor>(@"
                    SELECT * FROM ClientUserContractor WHERE ID=@ID",
                    new { ID = id });

                if (!string.IsNullOrEmpty(result.R_B_BDATE))
                    result.R_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(result.R_B_BDATE.Substring(0, 3)) + 1911}-{result.R_B_BDATE.Substring(3, 2)}-{result.R_B_BDATE.Substring(5, 2)}");

                return result;
            }
        }

        /// <summary>
        /// 取得使用者所有的承包商
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        public List<ClientUserContractor> GetContractorByUser(ClientUserContractor filter)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<ClientUserContractor>(@"
                    SELECT *
                    FROM ClientUserContractor
                    WHERE (@R_G_NO='' OR R_G_NO=@R_G_NO)
                        AND (@R_NAME='' OR R_NAME LIKE '%'+@R_NAME+'%')
                        AND (@R_B_NAM='' OR R_B_NAM LIKE '%'+@R_B_NAM+'%')
                        AND (@R_M_NAM='' OR R_M_NAM LIKE '%'+@R_M_NAM+'%')
                        AND (@R_C_NAM='' OR R_C_NAM LIKE '%'+@R_C_NAM+'%')
                        AND ClientUserID=@ClientUserID",
                        new
                        {
                            R_G_NO = filter.R_G_NO ?? "",
                            R_NAME = filter.R_NAME ?? "",
                            R_B_NAM = filter.R_B_NAM ?? "",
                            R_M_NAM = filter.R_M_NAM ?? "",
                            R_C_NAM = filter.R_C_NAM ?? "",
                            ClientUserID = filter.ClientUserID
                        }).ToList();

                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.R_B_BDATE))
                        item.R_B_BDATE2 = Convert.ToDateTime($"{Convert.ToInt32(item.R_B_BDATE.Substring(0, 3)) + 1911}-{item.R_B_BDATE.Substring(3, 2)}-{item.R_B_BDATE.Substring(5, 2)}");
                }

                return result;
            }
        }

        /// <summary>
        /// 新增承包商
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddContractor(ClientUserContractor contractor)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(contractor);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddContractor: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 修改承包商
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool UpdateContractor(ClientUserContractor contractor)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(contractor);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateContractor: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 刪除承包商
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool DeleteContractor(ClientUserContractor contractor)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Execute(@"
                        DELETE FROM ClientUserContractor
                        WHERE ID=@ID AND ClientUserID=@ClientUserID",
                        new { ID = contractor.ID, ClientUserID = contractor.ClientUserID });
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"DeleteContractor: {ex.StackTrace}|{ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
