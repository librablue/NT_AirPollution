using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Access
{
    /// <summary>
    /// 繳費資料
    /// </summary>
    public class ABUDF_1
    {
        /// <summary>
        /// 管制編號
        /// </summary>
        public string C_NO { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        public int? SER_NO { get; set; }
        /// <summary>
        /// 期別
        /// </summary>
        public string P_TIME { get; set; }
        /// <summary>
        /// 填發日期
        /// </summary>
        public string P_DATE { get; set; }
        /// <summary>
        /// 繳費期限
        /// </summary>
        public string E_DATE { get; set; }
        /// <summary>
        /// 對帳日期
        /// </summary>
        public string F_DATE { get; set; }
        /// <summary>
        /// 聯單序號
        /// </summary>
        public string FLNO { get; set; }
        /// <summary>
        /// 繳費金額
        /// </summary>
        public double F_AMT { get; set; }
        /// <summary>
        /// 退費金額
        /// </summary>
        public double B_AMT { get; set; }
        /// <summary>
        /// 繳費日期
        /// </summary>
        public string PM_DATE { get; set; }
        /// <summary>
        /// 入庫日期
        /// </summary>
        public string A_DATE { get; set; }
        /// <summary>
        /// 備註
        /// </summary>
        public string COMM { get; set; }
        /// <summary>
        /// 資料異動職工編號
        /// </summary>
        public string KEYIN { get; set; }
        /// <summary>
        /// 建立日期
        /// </summary>
        public DateTime C_DATE { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime M_DATE { get; set; }
    }
}
