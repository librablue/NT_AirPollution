using NT_AirPollution.Admin.Models;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    public class DownloadController : ApiController
    {
        private readonly string domainName = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly string downloadPath = ConfigurationManager.AppSettings["DownloadPath"].ToString();
        private readonly DownloadService downloadService = new DownloadService();

        public List<Downloads> GetDownloads()
        {
            var downloads = downloadService.GetDownloads();
            foreach (var item in downloads)
            {
                item.Link = $"{domainName}/static/download/{item.Filename}";
            }

            return downloads;
        }

        public Downloads GetDownload(int id)
        {
            var download = downloadService.GetDownload(id);
            download.Link = $"{domainName}/static/download/{download.Filename}";

            return download;
        }

        public HttpResponseMessage AddDownload(Download download)
        {
            var response = new HttpResponseMessage();
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                download.CreateDate = DateTime.Now;
                download.ModifyDate = null;
                download.DeleteDate = null;
                downloadService.AddDownload(download);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("系統發生異常，請稍候再試。");
            }

            return response;
        }

        public HttpResponseMessage UpdateDownload(Download download)
        {
            var response = new HttpResponseMessage();
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception();
                }

                downloadService.UpdateDownload(download);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("系統發生異常，請稍候再試。");
            }

            return response;
        }

        [HttpPost]
        public HttpResponseMessage DeleteDownload(dynamic obj)
        {
            var response = new HttpResponseMessage();
            try
            {
                int id = Convert.ToInt32(obj.id);
                downloadService.DeleteDownload(id);
                response.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Content = new StringContent("系統發生異常，請稍候再試。");
            }

            return response;
        }

        /// <summary>
        /// 上傳圖片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public UploadResponse Upload()
        {
            UploadResponse res = new UploadResponse();
            try
            {
                if (HttpContext.Current.Request.Files.Count == 0)
                    throw new Exception();

                // 取得檔案
                HttpPostedFile file = HttpContext.Current.Request.Files[0];
                // 設定儲存路徑
                string savePath = $"{downloadPath}/{file.FileName}";
                string webPath = $"{domainName}/static/download/{file.FileName}";
                // 儲存
                file.SaveAs(savePath);

                res.uploaded = 1;
                res.url = webPath;
                res.filename = file.FileName;
                return res;
            }
            catch (Exception ex)
            {
                res.uploaded = 0;
                res.url = "";
                return res;
            }
        }
    }
}
