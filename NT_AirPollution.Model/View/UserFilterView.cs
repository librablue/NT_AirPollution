using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    public class UserFilterView
    {
        public string Text { get; set; }
        public int? RoleID { get; set; }
        public bool? Enabled { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
