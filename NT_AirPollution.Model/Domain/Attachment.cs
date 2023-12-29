using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Attachment")]
    public class Attachment
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public string File1 { get; set; }
        public string File2 { get; set; }
        public string File3 { get; set; }
        public string File4 { get; set; }
        public string File5 { get; set; }
        public string File6 { get; set; }
        public string File7 { get; set; }
        public string File8 { get; set; }
    }
}
