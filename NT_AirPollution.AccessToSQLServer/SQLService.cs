using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessToSQLServer
{
    public class SQLService : BaseService
    {
        public IEnumerable<ClientUser> GetClientUser()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.GetAll<ClientUser>();
                return result;
            }
        }

        public IEnumerable<TDMFORMA> GetTDMFORMA()
        {
            using (var cn = new SqlConnection(connStr))
            {
                var result = cn.Query<TDMFORMA>(@"
                    SELECT DSG_EUSR_NAME, C_NO, SER_NO FROM TDMFORMA
                    WHERE C_NO<>'' AND SER_NO<>''");
                return result;
            }
        }

        public long AddForm(Form form)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long id = cn.Insert(form);
                    return id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public long AddFormB(FormB formB)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long id = cn.Insert(formB);
                    return id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public long AddPayment(Payment payment)
        {
            using (var cn = new SqlConnection(connStr))
            {
                try
                {
                    long id = cn.Insert(payment);
                    return id;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
