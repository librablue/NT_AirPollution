using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class DownloadController : Controller
    {
        private readonly DownloadService downloadService = new DownloadService();

        public ActionResult Index()
        {
            var downloads = downloadService.GetDownloads();
            return View(downloads);
        }
    }
}