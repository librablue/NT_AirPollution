using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    public class AirController : ApiController
    {
        private readonly AirService _airService = new AirService();

        [HttpPost]
        public List<AirReport> GetAirs(AirFilter filter)
        {
            return _airService.GetAirs(filter);
        }
    }
}
