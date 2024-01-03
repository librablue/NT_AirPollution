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
                    //HttpCookie memberCookie = Context.Request.Cookies["MEMBER"];
                    //HttpCookie nonmemberCookie = Context.Request.Cookies["NONMEMBER"];
                    //if (memberCookie == null && nonmemberCookie == null)
                    //    return;

                    //FormsAuthenticationTicket memberTicket = FormsAuthentication.Decrypt(memberCookie.Value);
                    //FormsAuthenticationTicket nonmemberTicket = FormsAuthentication.Decrypt(nonmemberCookie.Value);
                    //UserData member = JsonConvert.DeserializeObject<UserData>(memberTicket.UserData);
                    //UserData nonmember = JsonConvert.DeserializeObject<UserData>(nonmemberTicket.UserData);

                    //List<string> roles = new List<string>();

                    //if (member != null)
                    //    roles.Add(member.Role);
                    //if (nonmember != null)
                    //    roles.Add(nonmember.Role);

                    //if (Context.User != null)
                    //    Context.User = new GenericPrincipal(Context.User.Identity, roles.ToArray());

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
