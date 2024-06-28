using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string Term { get; set; }
        public DateTime PayEndDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string PaymentID { get; set; }
        public double? PayAmount { get; set; }
        public DateTime? PayDate { get; set; }
        public DateTime CreateDate { get; set; }
        public string BankLog { get; set; }
    }
}
