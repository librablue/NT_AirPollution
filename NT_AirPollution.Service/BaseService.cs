using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace NT_AirPollution.Service
{
    public class BaseService
    {
        protected readonly string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;
        protected readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        public static ClientUser CurrentUser
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    ClientUser clientUser = JsonConvert.DeserializeObject<ClientUser>(ticket.UserData);
                    return clientUser;
                }

                return null;
            }
        }
    }
}
