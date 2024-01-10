using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize(Roles = "Member2")]
    public class ManageController : BaseController
    {
        private readonly FormService _formService = new FormService();

        public ActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetForms(FormFilter filter)
        {
            filter.CompanyID = BaseService.CurrentUser.CompanyID;
            var result = _formService.GetFormsByCompany(filter);
            return Json(new AjaxResult { Status = true, Message = result });
        }

        public JsonResult GetFormByID(long id)
        {
            try
            {
                // 檢查是否為自己的申請單
                var filter = new FormFilter
                {
                    CompanyID = BaseService.CurrentUser.CompanyID
                };
                var result = _formService.GetFormsByCompany(filter).FirstOrDefault(o => o.ID == id);
                if (result == null || (result.S_G_NO != BaseService.CurrentUser.CompanyID && result.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                return Json(new AjaxResult { Status = true, Message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}