using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("RefundBank")]
    public class RefundBank
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string Code { get; set; }
        public string Account { get; set; }
        public string Photo { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
