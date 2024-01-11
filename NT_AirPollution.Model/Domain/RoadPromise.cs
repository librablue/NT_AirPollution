using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("RoadPromise")]
    public class RoadPromise
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
