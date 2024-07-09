using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class OptionController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly OptionService _optionService = new OptionService();
        public JsonResult GetDistrict()
        {
            var result = _optionService.GetDistrict();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProjectCode()
        {
            var result = _optionService.GetProjectCode();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttachmentInfo()
        {
            var result = _optionService.GetAttachmentInfo();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="f">原始檔名</param>
        /// <param name="f">下載檔名</param>
        /// <returns></returns>
        public FileResult Download(string f, string n)
        {
            string fileName = $@"{_uploadPath}\{f}";
            if (!System.IO.File.Exists(fileName))
                return null;

            byte[] fileBytes = System.IO.File.ReadAllBytes(fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, n ?? f);
        }
    }
}