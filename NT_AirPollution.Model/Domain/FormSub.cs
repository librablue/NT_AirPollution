using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("FormSub")]
    public class FormSub
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string COMP_NAM { get; set; }
        public string ADDR { get; set; }
        public double AREA { get; set; }
        public string B_DATE { get; set; }
        public string E_DATE { get; set; }
    }
}
