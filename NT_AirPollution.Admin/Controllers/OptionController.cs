using ClosedXML.Excel;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    public class OptionController : ApiController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly OptionService _optionService = new OptionService();

        public List<District> GetDistrict()
        {
            return _optionService.GetDistrict();
        }

        public List<ProjectCode> GetProjectCode()
        {
            return _optionService.GetProjectCode();
        }

        public List<AttachmentInfo> GetAttachmentInfo()
        {
            return _optionService.GetAttachmentInfo();
        }

        [HttpGet]
        public HttpResponseMessage Download(string f)
        {
            try
            {
                string filePath = $@"{_uploadPath}\{f}";
                if (!System.IO.File.Exists(filePath))
                    return null;

                var stream = new FileStream(filePath, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = f
                };

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
