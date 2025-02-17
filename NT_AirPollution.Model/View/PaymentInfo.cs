using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    /// <summary>
    /// 繳費相關資訊
    /// </summary>
    public class PaymentInfo
    {
        /// <summary>
        /// 是否為公共工程
        /// </summary>
        public bool IsPublic { get; set; }
        /// <summary>
        /// 申請日期
        /// </summary>
        public DateTime ApplyDate { get; set; }
        /// <summary>
        /// 審核日期
        /// </summary>
        public DateTime VerifyDate { get; set; }
        /// <summary>
        /// 繳費期限
        /// </summary>
        public DateTime PayEndDate { get; set; }
        /// <summary>
        /// 開工日期
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 總金額
        /// </summary>
        public double TotalPrice { get; set; }
        /// <summary>
        /// 本次繳費金額
        /// </summary>
        public double CurrentPrice { get; set; }
        /// <summary>
        /// 延遲天數
        /// </summary>
        public int DelayDays { get; set; }
        /// <summary>
        /// 滯納金
        /// </summary>
        public double Penalty { get; set; }
        /// <summary>
        /// 利息
        /// </summary>
        public double Interest { get; set; }
        /// <summary>
        /// 郵局儲匯局定存利率
        /// </summary>
        public double Rate { get; set; }
    }
}
