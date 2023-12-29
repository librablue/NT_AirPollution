using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class OptionController : BaseController
    {
        private readonly OptionService _optionService = new OptionService();
        public JsonResult GetDistrict()
        {
            var result = _optionService.GetDistrict();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}