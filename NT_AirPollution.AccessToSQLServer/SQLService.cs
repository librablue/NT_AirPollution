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
