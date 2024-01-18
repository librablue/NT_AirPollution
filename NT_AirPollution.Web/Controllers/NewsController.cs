using NT_AirPollution.Service;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly NewsService _newsService = new NewsService();
        private readonly int pageSize = 10;

        public ActionResult Index(int page = 1)
        {
            int currentPage = page < 1 ? 1 : page;
            var news = _newsService.GetNews();
            var result = news.ToPagedList(currentPage, pageSize);
            return View(result);
        }

        public ActionResult Content(long id)
        {
            var result = _newsService.GetNews(id);
            if (result == null)
                return RedirectToAction("Index", "News");

            return View(result);
        }
    }
}