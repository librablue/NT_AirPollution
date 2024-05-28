using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("AdminRole")]
    public class AdminRole
    {
        [ExplicitKey]
        public long ID { get; set; }
        public string RoleName { get; set; }
    }
}
