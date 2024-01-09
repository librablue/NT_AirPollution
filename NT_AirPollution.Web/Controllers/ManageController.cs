using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize(Roles = "Member2")]
    public class ManageController : BaseController
    {
        public ActionResult Form()
        {
            return View();
        }
    }
}