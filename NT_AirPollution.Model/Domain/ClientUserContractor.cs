using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("ClientUserContractor")]
    public class ClientUserContractor
    {
        [Key]
        public long ID { get; set; }
        public long ClientUserID { get; set; }
        public string R_NAME { get; set; }
        public string R_G_NO { get; set; }
        public string R_ADDR1 { get; set; }
        public string R_ADDR2 { get; set; }
        public string R_TEL { get; set; }
        public string R_B_NAM { get; set; }
        public string R_B_TIT { get; set; }
        public string R_B_ID { get; set; }
        public string R_B_BDATE { get; set; }
        /// <summary>
        /// 西元年
        /// </summary>
        public DateTime? R_B_BDATE2 { get; set; }
        public string R_ADDR3 { get; set; }
        public string R_TEL1 { get; set; }
        public string R_M_NAM { get; set; }
        public string R_C_NAM { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ModifyDate { get; set; }
    }
}
