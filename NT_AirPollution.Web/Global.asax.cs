using Newtonsoft.Json;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace NT_AirPollution.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request.IsAuthenticated)
            {
                try
                {
                    HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
                    if (authCookie == null || authCookie.Value == "")
                        return;

                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    UserData clientUser = JsonConvert.DeserializeObject<UserData>(ticket.UserData);
                    if (Context.User != null)
                        Context.User = new GenericPrincipal(Context.User.Identity, clientUser.Role);
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
