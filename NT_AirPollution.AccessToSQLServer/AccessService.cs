using Dapper;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessToSQLServer
{
    public class AccessService : BaseService
    {
        private readonly string domain = "192.168.96.16";
        private readonly string userName = "cpc";
        private readonly string password = "Ciohe@2565!";

        /// <summary>
        /// 取得全部ABUDF
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ABUDF> GetABUDF()
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.Query<ABUDF>("SELECT * FROM ABUDF");
                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得單筆ABUDF
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <returns></returns>
        public ABUDF GetABUDF(string c_no, int ser_no)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault<ABUDF>(@"
                        SELECT * FROM ABUDF WHERE C_NO=@C_NO AND SER_NO=@SER_NO",
                    new { C_NO = c_no, SER_NO = ser_no });

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得全部ABUDF_1
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public IEnumerable<ABUDF_1> GetABUDF_1()
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.Query<ABUDF_1>("SELECT * FROM ABUDF_1");
                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得單筆ABUDF_1
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ABUDF_1 GetABUDF_1(string c_no, int ser_no, string term)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault<ABUDF_1>(@"
                        SELECT * FROM ABUDF_1 WHERE C_NO=@C_NO AND SER_NO=@SER_NO AND P_TIME=@P_TIME",
                    new { C_NO = c_no, SER_NO = ser_no, P_TIME = term });

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得全部ABUDF_I
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ABUDF_I> GetABUDF_I()
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.Query<ABUDF_I>("SELECT * FROM ABUDF_I");
                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得單筆ABUDF_I
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public ABUDF_I GetABUDF_I(string c_no, int ser_no, string term)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault<ABUDF_I>(@"
                        SELECT * FROM ABUDF_I WHERE C_NO=@C_NO AND SER_NO=@SER_NO AND P_TIME=@P_TIME",
                    new { C_NO = c_no, SER_NO = ser_no, P_TIME = term });

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得全部ABUDF_B
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ABUDF_B> GetABUDF_B()
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.Query<ABUDF_B>(@"SELECT * FROM ABUDF_B");
                return result;
            }
#if !DEBUG
            }
#endif
        }
    }
}
