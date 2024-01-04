using NT_AirPollution.Model.Domain;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http;

namespace NT_AirPollution.Admin.ActionFilter
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            AdminUser user = BaseService.CurrentAdmin;
            return user != null;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!IsAuthorized(actionContext))
            {
                HandleUnauthorizedRequest(actionContext);
            }
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            HttpResponseMessage result = new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Unauthorized,
                RequestMessage = actionContext.Request
            };

            actionContext.Response = result;
        }
    }
}