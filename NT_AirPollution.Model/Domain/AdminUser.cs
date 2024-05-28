using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("AdminUser")]
    public class AdminUser
    {
        [Key]
        public long ID { get; set; }
        public long RoleID { get; set; }
        public string UserName { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public bool Enabled { get; set; }
        [Computed]
        public string Captcha { get; set; }
    }
}
