using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        /// <summary>
        /// 期數
        /// </summary>
        public string Term { get; set; }
        /// <summary>
        /// 繳費期限
        /// </summary>
        public DateTime PayEndDate { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        public string PaymentID { get; set; }
        /// <summary>
        /// 應繳金額
        /// </summary>
        public double PayableAmount { get; set; }
        /// <summary>
        /// 滯納金
        /// </summary>
        public double? Penalty { get; set; }
        /// <summary>
        /// 利息
        /// </summary>
        public double? Interest { get; set; }
        /// <summary>
        /// 年利率
        /// </summary>
        public double Percent { get; set; }
        /// <summary>
        /// 繳費金額
        /// </summary>
        public double? PayAmount { get; set; }
        /// <summary>
        /// 繳費日期
        /// </summary>
        public DateTime? PayDate { get; set; }
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 入帳日期
        /// </summary>
        public DateTime? ModifyDate { get; set; }
        public string BankLog { get; set; }
    }
}
