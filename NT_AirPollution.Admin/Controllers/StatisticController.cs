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
    public class StatisticController : ApiController
    {
        private readonly FormService _formService = new FormService();
        private readonly ClientUserService _clientUserService = new ClientUserService();

        public long GetUserCount(DateTime sdate, DateTime edate)
        {
            return _clientUserService.GetUserCount(sdate, edate);
        }

        public long GetFormsCount(DateTime sdate, DateTime edate)
        {
            return _formService.GetFormsCount(sdate, edate);
        }

        public long GetPaymentCount(DateTime sdate, DateTime edate)
        {
            return _formService.GetPaymentCount(sdate, edate);
        }

        public double GetCarbon(DateTime sdate, DateTime edate)
        {
            return _formService.GetCarbon(sdate, edate);
        }
    }
}
