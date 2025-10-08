using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    [Table("Form")]
    public class FormView : Form
    {
        [Computed]
        public RefundBank RefundBank { get; set; } = new RefundBank();
        [Computed]
        public List<Payment> Payments { get; set; } = new List<Payment>();
        [Computed]
        public List<StopWork> StopWorks { get; set; } = new List<StopWork>();
        [Computed]
        public FormB FormB { get; set; };
        [Computed]
        public string Captcha { get; set; }
        [Computed]
        public WorkStatus WorkStatus { get; set; }
        [Computed]
        public Commitment Commitment { get; set; }

        /// <summary>
        /// 是否寄送郵件通知
        /// </summary>
        [Computed]
        public bool IsMailFormStatus { get; set; }

        /// <summary>
        /// 是否寄送郵件通知
        /// </summary>
        [Computed]
        public bool IsMailCalcStatus { get; set; }
    }
}
