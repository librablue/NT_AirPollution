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

        public List<InterestRate> GetRates()
        {
            return _optionService.GetRates();
        }

        public List<AdminRole> GetAdminRoles()
        {
            var result = _optionService.GetAdminRoles();
            return result;
        }

        public bool AddRate(InterestRate rate)
        {
            try
            {
                _optionService.AddRate(rate);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool DeleteRate(InterestRate rate)
        {
            try
            {
                _optionService.DeleteRate(rate);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
