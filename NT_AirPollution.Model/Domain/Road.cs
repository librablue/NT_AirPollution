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
        public int Length { get; set; }
    }
}
