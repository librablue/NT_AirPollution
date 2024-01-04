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
    public class NewsController : ApiController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _staticPath = ConfigurationManager.AppSettings["StaticPath"].ToString();
        private readonly NewsService newsService = new NewsService();
        public List<News> GetNews()
        {
            var news = newsService.GetNews().OrderByDescending(o => o.CreateDate).ToList();
            return news;
        }

        [HttpPost]
        public bool AddNews(News news)
        {
            try
            {
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
                string absoluteDirPath = $"{_uploadPath}/News";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                // 設定儲存路徑
                string savePath = $"{absoluteDirPath}/Raw/{fileName}{Path.GetExtension(file.FileName)}";
                string compressPath = $"{absoluteDirPath}/{fileName}{Path.GetExtension(file.FileName)}";
                string webPath = $"{_staticPath}/News/{fileName}{Path.GetExtension(file.FileName)}";
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
