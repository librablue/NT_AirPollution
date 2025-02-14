using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("InterestRate")]
    public class InterestRate
    {
        [Key]
        public long ID { get; set; }
        public DateTime Date { get; set; }
        public double Rate { get; set; }
    }
}
