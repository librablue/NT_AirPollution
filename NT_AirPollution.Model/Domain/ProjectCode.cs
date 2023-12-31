using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("ProjectCode")]
    public class ProjectCode
    {
        [Key]
        public long ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
