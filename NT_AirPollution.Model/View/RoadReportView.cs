using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace NT_AirPollution.Model.View
{
    public class RoadReportView
    {
        public long FormID { get; set; }
        public long PromiseID { get; set; }
        public int YearMth { get; set; }
        [Computed]
        public List<Road> Roads { get; set; } = new List<Road>();
    }
}
