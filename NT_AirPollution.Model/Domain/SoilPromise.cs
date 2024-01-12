using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("SoilPromise")]
    public class SoilPromise
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public DateTime StartDate { get; set; }
        public int DigDays { get; set; }
        public int Option1 { get; set; }
        public int? Option2 { get; set; }
        public string Other1 { get; set; }
        public string Reason { get; set; }
        public string Other2 { get; set; }
        public long CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
