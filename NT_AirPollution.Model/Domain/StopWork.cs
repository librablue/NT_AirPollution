using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("StopWork")]
    public class StopWork
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string DOWN_DATE { get; set; }
        [Computed]
        public DateTime DOWN_DATE2 { get; set; }
        public string UP_DATE { get; set; }
        [Computed]
        public DateTime UP_DATE2 { get; set; }
        public int DOWN_DAY { get; set; }
        public DateTime? C_DATE { get; set; }
        public DateTime? M_DATE { get; set; }
    }
}
