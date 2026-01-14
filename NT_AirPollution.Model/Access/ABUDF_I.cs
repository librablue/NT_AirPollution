using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Access
{
    public class ABUDF_I
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
        /// 逾期起始日期
        /// </summary>
        public string S_DATE { get; set; }
        /// <summary>
        /// 逾期終止日期
        /// </summary>
        public string E_DATE { get; set; }
        /// <summary>
        /// 年利率%
        /// </summary>
        public double PERCENT { get; set; }
        /// <summary>
        /// 本期應繳金額
        /// </summary>
        public double F_AMT { get; set; }
        /// <summary>
        /// 利息
        /// </summary>
        public double I_AMT { get; set; }
        /// <summary>
        /// 滯納金
        /// </summary>
        public double PEN_AMT { get; set; }
        /// <summary>
        /// 滯納金利率
        /// </summary>
        public double? PEN_RATE { get; set; }
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
