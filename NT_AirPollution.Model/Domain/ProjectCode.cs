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
        [ExplicitKey]
        public string ID { get; set; }
        public string Kind { get; set; }
        public string Name { get; set; }
        public int Level1 { get; set; }
        public int Level2 { get; set; }
        public double Rate1 { get; set; }
        public double Rate2 { get; set; }
        public double Rate3 { get; set; }
        public string Fomula { get; set; }
    }
}
