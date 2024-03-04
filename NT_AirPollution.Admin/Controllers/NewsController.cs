using NT_AirPollution.Admin.ActionFilter;
using NT_AirPollution.Admin.Helper;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    [AuthorizeUser]
    public class NewsController : ApiController
    {
        private readonly string _staticUploadPath = ConfigurationManager.AppSettings["StaticUploadPath"].ToString();
        private readonly string _staticPath = ConfigurationManager.AppSettings["StaticPath"].ToString();
        private readonly NewsService newsService = new NewsService();
        public List<News> GetNews()
        {
            var news = newsService.GetNews().OrderByDescending(o => o.PublishDate).ToList();
            return news;
        }

        [HttpPost]
        public bool AddNews(News news)
        {
            try
            {
                news.CreateDate = DateTime.Now;
                newsService.AddNews(news);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool UpdateNews(News news)
        {
            try
            {
                newsService.UpdateNews(news);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool DeleteNews(News news)
        {
            try
            {
                news.DeleteDate = DateTime.Now;
                newsService.UpdateNews(news);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// 上傳圖片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public string UploadFile()
        {
            try
            {
                if (HttpContext.Current.Request.Files.Count == 0)
                {
                    throw new Exception("尚未選擇上傳檔案");
                }

                // 取得檔案
                HttpPostedFile file = HttpContext.Current.Request.Files[0];

                string fileName = Guid.NewGuid().ToString();

                // 設定資料夾
                string absoluteDirPath = $"{_staticUploadPath}/News";


                // 設定儲存路徑
                string savePath = $@"{_staticUploadPath}\News\Raw\{fileName}{Path.GetExtension(file.FileName)}";
                string compressPath = $@"{_staticUploadPath}\News\{fileName}{Path.GetExtension(file.FileName)}";
                string webPath = $"{_staticPath}/News/{fileName}{Path.GetExtension(file.FileName)}";

                string directoryPath = Path.GetDirectoryName(savePath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                // 儲存檔案
                file.SaveAs(savePath);
                // 壓縮圖片
                ImageHelper.CompressImage(savePath, compressPath, 50);

                return webPath;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
