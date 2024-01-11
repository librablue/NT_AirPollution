using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("RoadReport")]
    public class RoadReport
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public long RoadID { get; set; }
        public int YearMth { get; set; }
        public string RoadName { get; set; }
        public string CleanWay1 { get; set; }
        public string CleanWay2 { get; set; }
        public string Frequency { get; set; }
        public int Times { get; set; }
        public int LengthPerTimes { get; set; }
        public int TotalLength { get; set; }
        public int GreenLength { get; set; }
        public long CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
