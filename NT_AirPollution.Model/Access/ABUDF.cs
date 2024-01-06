using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NT_AirPollution.Model.Access
{
    public class ABUDF
    {
        public string C_NO { get; set; }
        public int? SER_NO { get; set; }
        public string COUNTRY { get; set; }
        public string TOWN_NO { get; set; }
        public string TOWN_NA { get; set; }
        public string KIND_NO { get; set; }
        public string KIND { get; set; }
        public string AP_DATE { get; set; }
        public string B_SERNO { get; set; }
        public bool? PUB_COMP { get; set; }
        public string S_NAME { get; set; }
        public string S_G_NO { get; set; }
        public string S_ADDR1 { get; set; }
        public string S_ADDR2 { get; set; }
        public string S_TEL { get; set; }
        public string S_B_NAM { get; set; }
        public string S_B_TIT { get; set; }
        public string S_B_ID { get; set; }
        public string S_B_BDATE { get; set; }
        public string S_C_NAM { get; set; }
        public string S_C_TIT { get; set; }
        public string S_C_ID { get; set; }
        public string S_C_ADDR { get; set; }
        public string S_C_TEL { get; set; }
        public string R_NAME { get; set; }
        public string R_G_NO { get; set; }
        public string R_ADDR1 { get; set; }
        public string R_ADDR2 { get; set; }
        public string R_TEL { get; set; }
        public string R_B_NAM { get; set; }
        public string R_B_TIT { get; set; }
        public string R_B_ID { get; set; }
        public string R_B_BDATE { get; set; }
        public string R_ADDR3 { get; set; }
        public string R_TEL1 { get; set; }
        public string R_M_NAM { get; set; }
        public string R_C_NAM { get; set; }
        public string A_KIND { get; set; }
        public double? AREA { get; set; }
        public double? VOLUMEL { get; set; }
        public double? RATIOLB { get; set; }
        public double? DENSITYL { get; set; }
        public double? MONEY { get; set; }
        public double? C_MONEY { get; set; }
        public double? PERCENT { get; set; }
        public string YEAR { get; set; }
        public string B_DATE { get; set; }
        public string E_DATE { get; set; }
        public double? S_AMT { get; set; }
        public string P_KIND { get; set; }
        public int? P_NUM { get; set; }
        public double? P_AMT { get; set; }
        public string FIN_DATE { get; set; }
        public string FIN_COM { get; set; }
        public string STATE { get; set; }
        public string ID_DOC1 { get; set; }
        public string ID_DOC2 { get; set; }
        public string ID_DOC3 { get; set; }
        public string COMP_DOC1 { get; set; }
        public string COMP_DOC2 { get; set; }
        public string COMP_DOC3 { get; set; }
        public string COMP_OTH { get; set; }
        public string BUD_DOC1 { get; set; }
        public string BUD_DOC2 { get; set; }
        public string BUD_DOC3 { get; set; }
        public string BUD_OTH { get; set; }
        public string REC_YN { get; set; }
        public double? AREA_B { get; set; }
        public double? AREA_F { get; set; }
        public double? PERC_B { get; set; }
        public string COMP_NAM { get; set; }
        public string ADDR { get; set; }
        public string AP_TYPE { get; set; }
        public double? UTME { get; set; }
        public double? UTMN { get; set; }
        public string LATLNG { get; set; }
        public double? AREA3 { get; set; }
        public double? AREA2 { get; set; }
        public double? O_C_Q { get; set; }
        public string G_NAME { get; set; }
        public string ADDR1 { get; set; }
        public string COMP_NAM1 { get; set; }
        public double? ENG_STONE { get; set; }
        public double? ENG_WRAP { get; set; }
        public string ENG_DES { get; set; }
        public string ENG_DES_TEL { get; set; }
        public string ENG_DES_ADDR { get; set; }
        public string NOLEVY { get; set; }
        public string COMP_L { get; set; }
        public string RCAP_DATE { get; set; }
        public string RC_DATE { get; set; }
        public string RC_SERNO { get; set; }
        public string RCE { get; set; }
        public string COMMENT { get; set; }
        public string EIACOMMENTS { get; set; }
        public DateTime? C_DATE { get; set; }
        public DateTime? M_DATE { get; set; }
        public string KEYIN { get; set; }
        public string RECCOMMENTS { get; set; }
    }
}
