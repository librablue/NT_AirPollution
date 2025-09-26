using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessToSQLServer
{
    public class BaseService
    {
        protected readonly string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;
        protected readonly string accessConnStr = ConfigurationManager.ConnectionStrings["Access"].ConnectionString;
    }
}
