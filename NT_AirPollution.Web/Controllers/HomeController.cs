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

        public ActionResult Index()
        {
            ViewBag.News = _newsService.GetNews().Take(5).ToList();
            ViewBag.Downloads = _downloadService.GetDownloads().Take(5).ToList();
            return View();
        }
    }
}