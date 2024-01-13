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
    public class RoadController : ApiController
    {
        private readonly RoadService _roadService = new RoadService();

        [HttpPost]
        public List<RoadExcel> GetRoadReport(RoadFilter filter)
        {
            return _roadService.GetRoadReport(filter);
        }
    }
}
