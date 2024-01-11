using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Road")]
    public class Road
    {
        [Key]
        public long ID { get; set; }
        public long PromiseID { get; set; }
        public string RoadName { get; set; }
        public double RoadLength { get; set; }
        public string CleanWay1 { get; set; }
        public string CleanWay2 { get; set; }
        public string Frequency { get; set; }
        public int Times { get; set; }
    }
}
