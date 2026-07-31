using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Access
{
    public class ABUDF_DAY
    {
        public long DAYSID { get; set; }
        /// <summary>
        /// 管制編號
        /// </summary>
        public string C_NO { get; set; }
        /// <summary>
        /// 序號
        /// </summary>
        public int? SER_NO { get; set; }
        /// <summary>
        /// 停工日期
        /// </summary>
        public string DOWN_DATE { get; set; }
        /// <summary>
        /// 復工日期
        /// </summary>
        public string UP_DATE { get; set; }
        /// <summary>
        /// 停工日數
        /// </summary>
        public string DOWN_DAY { get; set; }

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
