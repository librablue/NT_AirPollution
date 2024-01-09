using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("AirFile")]
    public class AirFile
    {
        [Key]
        public long ID { get; set; }
        public long AirID { get; set; }
        public string FileName { get; set; }
    }
}
