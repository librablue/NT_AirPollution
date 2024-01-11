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
        public bool? Agree1 { get; set; }
        public bool? Agree1_1 { get; set; }
        public bool? Agree1_2 { get; set; }
        public bool? Agree1_3 { get; set; }
        public string Agree1_3_Text { get; set; }
        public bool? Agree2 { get; set; }
        public bool? Agree2_1 { get; set; }
        public bool? Agree2_2 { get; set; }
        public string Agree2_2_Text { get; set; }
        public bool? Agree2_3 { get; set; }
        public string Agree2_3_Text { get; set; }
        public long CreateUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }

}
