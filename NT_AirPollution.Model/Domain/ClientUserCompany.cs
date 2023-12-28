using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("ClientUserCompany")]
    public class ClientUserCompany
    {
        [Key]
        public long ID { get; set; }
        public long ClientUserID { get; set; }
        public string S_NAME { get; set; }
        public string S_G_NO { get; set; }
        public string S_ADDR1 { get; set; }
        public string S_ADDR2 { get; set; }
        public string S_TEL { get; set; }
        public string S_B_NAM { get; set; }
        public string S_B_TIT { get; set; }
        public string S_B_ID { get; set; }
        public string S_B_BDATE { get; set; }
        /// <summary>
        /// 西元年
        /// </summary>
        [Computed]
        public DateTime? S_B_BDATE2 { get; set; }
        public string S_C_NAM { get; set; }
        public string S_C_TIT { get; set; }
        public string S_C_ID { get; set; }
        public string S_C_ADDR { get; set; }
        public string S_C_TEL { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
