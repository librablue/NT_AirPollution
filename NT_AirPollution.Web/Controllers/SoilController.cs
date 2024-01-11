using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize(Roles = "Member2")]
    public class SoilController : BaseController
    {
        private readonly SoilService _soilService = new SoilService();
        private readonly FormService _formService = new FormService();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPromiseByForm(long formID)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(formID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var promise = _soilService.GetPromiseByFormID(formID);
                return Json(new AjaxResult { Status = true, Message = promise }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddPromise(SoilPromise promise)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(promise.FormID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var promiseInDB = _soilService.GetPromiseByFormID(promise.FormID);
                if (promiseInDB != null)
                    throw new Exception("禁止重複建立承諾書");

                promise.FormID = formInDB.ID;
                promise.CreateUserID = BaseService.CurrentUser.ID;
                promise.CreateDate = DateTime.Now;
                _soilService.AddPromise(promise);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}