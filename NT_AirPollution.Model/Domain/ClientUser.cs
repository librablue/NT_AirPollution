using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("ClientUser")]
    public class ClientUser
    {
        [Key]
        public long ID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ActiveCode { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
