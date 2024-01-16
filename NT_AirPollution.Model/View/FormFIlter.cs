using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NT_AirPollution.Model.View
{
    public class FormFilter
    {
        public string C_NO { get; set; }
        public bool? PUB_COMP { get; set; }
        public string COMP_NAM { get; set; }
        public string CreateUserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long ClientUserID { get; set; }
        public string CreateUserEmail { get; set; }
        public FormStatus Status { get; set; }
        /// <summary>
        /// 統一編號
        /// </summary>
        public string CompanyID { get; set; }
        public WorkStatus WorkStatus { get; set; }
        public Commitment Commitment { get; set; }
    }
}
