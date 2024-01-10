using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize(Roles = "Member2")]
    public class RoadController : BaseController
    {
        private readonly RoadService _roadService = new RoadService();
        private readonly FormService _formService = new FormService();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPromiseByForm(int formID)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(formID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var airs = _roadService.GetPromiseByFormID(formID);
                return Json(new AjaxResult { Status = true, Message = airs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}