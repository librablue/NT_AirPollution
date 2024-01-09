using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
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
        private readonly FormService _formService = new FormService();

        [HttpPost]
        public JsonResult GetForms(FormFilter filter)
        {
            filter.CompanyID = BaseService.CurrentUser.CompanyID;
            var result = _formService.GetFormsByCompany(filter);
            return Json(new AjaxResult { Status = true, Message = result });
        }
    }
}