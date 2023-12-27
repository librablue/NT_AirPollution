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
        /// 新增驗證碼記錄
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool AddVerifyLog(VerifyLog log)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Insert(log);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 註冊檢查驗證碼
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public VerifyLog CheckVerifyLog(string email)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.QueryFirstOrDefault<VerifyLog>(@"
                    SELECT TOP 1 * FROM VerifyLog
                    WHERE Email=@Email
                        AND ActiveDate IS NULL
                    ORDER BY CreateDate DESC",
                    new { Email = email });
                return result;
            }
        }

        /// <summary>
        /// 修改驗證碼記錄
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool UpdateVerifyLog(VerifyLog log)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    cn.Update(log);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

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
        /// 新增使用者
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
                    Logger.Error(ex.Message);
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
        public bool UpdateUser(ClientUser user)
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
                    Logger.Error(ex.Message);
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 前台登入
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ClientUser SignIn(ClientUser user)
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
        /// 取得使用者所有的營建業主
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public List<ClientUserCompany> GetCompanyByUser(ClientUserCompany company)
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
                            S_G_NO = company.S_G_NO ?? "",
                            S_NAME = company.S_NAME ?? "",
                            S_B_NAM = company.S_B_NAM ?? "",
                            S_C_NAM = company.S_C_NAM ?? "",
                            ClientUserID = BaseService.CurrentUser.ID
                        }).ToList();

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
                    Logger.Error(ex.Message);
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
                    Logger.Error(ex.Message);
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
                    cn.Delete(company);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        /// <summary>
        /// 取得使用者所有的承包商
        /// </summary>
        /// <param name="contractor"></param>
        /// <returns></returns>
        public List<ClientUserContractor> GetContractorByUser(ClientUserContractor contractor)
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<ClientUserContractor>(@"
                    SELECT *
                    FROM ClientUserContractor
                    WHERE (@R_G_NO='' OR R_G_NO=@R_G_NO)
                        AND (@R_NAME='' OR R_NAME LIKE '%'+@R_NAME+'%')
                        AND (@R_B_NAM='' OR R_B_NAM LIKE '%'+@R_B_NAM+'%')
                        AND (@R_C_NAM='' OR R_C_NAM LIKE '%'+@R_C_NAM+'%')
                        AND ClientUserID=@ClientUserID",
                        new
                        {
                            R_G_NO = contractor.R_G_NO ?? "",
                            R_NAME = contractor.R_NAME ?? "",
                            R_B_NAM = contractor.R_B_NAM ?? "",
                            R_C_NAM = contractor.R_C_NAM ?? "",
                            ClientUserID = BaseService.CurrentUser.ID
                        }).ToList();

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
                    Logger.Error(ex.Message);
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
                    Logger.Error(ex.Message);
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
                    cn.Delete(contractor);
                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
