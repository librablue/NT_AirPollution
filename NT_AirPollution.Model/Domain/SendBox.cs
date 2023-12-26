using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("SendBox")]
    public class SendBox
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachment { get; set; }
        public int FailTimes { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
