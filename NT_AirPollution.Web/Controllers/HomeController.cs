using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly NewsService _newsService =  new NewsService();
        private readonly DownloadService _downloadService = new DownloadService();
        private readonly FormService _formService = new FormService();

        public ActionResult Index()
        {
            ViewBag.News = _newsService.GetNews().Take(5).ToList();
            ViewBag.Downloads = _downloadService.GetDownloads().Take(5).ToList();
            return View();
        }

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