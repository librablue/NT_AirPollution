using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class VerifyController : Controller
    {
        private readonly VerifyService _verifyService = new VerifyService();
        public ActionResult Index()
        {
            if (Request.QueryString["code"] == null)
                return View();

            string code = Request.QueryString["code"];
            var form = _verifyService.CheckActiveCode(code);
            if (form == null)
                return View();

            if (!form.IsActive)
            {
                // 更新為驗證通過
                _verifyService.VerifyForm(form.ID);
            }

            return View(form);
        }
    }
}