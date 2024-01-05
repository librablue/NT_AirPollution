using NT_AirPollution.Model.Domain;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    public class OptionController : ApiController
    {
        private readonly OptionService _optionService = new OptionService();

        public List<District> GetDistrict()
        {
            return _optionService.GetDistrict();
        }

        public List<ProjectCode> GetProjectCode()
        {
            return _optionService.GetProjectCode();
        }
    }
}
