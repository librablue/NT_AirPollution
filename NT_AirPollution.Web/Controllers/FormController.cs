using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class FormController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}