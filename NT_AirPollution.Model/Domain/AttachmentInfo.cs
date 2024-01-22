using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("AttachmentInfo")]
    public class AttachmentInfo
    {
        [Key]
        public long ID { get; set; }
        public bool PUB_COMP { get; set; }
        public string FileTitle { get; set; }
        public string Description { get; set; }
    }
}
