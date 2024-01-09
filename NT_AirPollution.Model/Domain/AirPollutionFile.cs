using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("AirPollutionFile")]
    public class AirPollutionFile
    {
        [Key]
        public long ID { get; set; }
        public string FileName { get; set; }
    }
}
