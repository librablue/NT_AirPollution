using hbehr.recaptcha;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class FormController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly ClientUserService _clientUserService = new ClientUserService();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();
        private readonly AccessService _accessService = new AccessService();
        private readonly VerifyService _verifyService = new VerifyService();

        public ActionResult Calc()
        {
            return View();
        }

        public ActionResult ProjectTable()
        {
            return View();
        }

        /// <summary>
        /// 試算金額
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public JsonResult GetTotalMoney(FormView form)
        {
            try
            {
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                var result = _formService.CalcTotalMoney(form, 0);
                return Json(new AjaxResult { Status = true, Message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = "計算過程發生異常" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}