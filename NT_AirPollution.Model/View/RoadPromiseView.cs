using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    [Table("RoadPromise")]
    public class RoadPromiseView : RoadPromise
    {
        [Computed]
        public List<Road> Roads { get; set; } = new List<Road>();
    }
}
