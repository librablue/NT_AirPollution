using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("PaymentProof")]
    public class PaymentProof
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string ProofFile { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
