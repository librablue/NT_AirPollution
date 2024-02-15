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
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();
        [Computed]
        public RefundBank RefundBank { get; set; } = new RefundBank();
        [Computed]
        public PaymentProof PaymentProof { get; set; } = new PaymentProof();
        [Computed]
        public List<StopWork> StopWorks { get; set; } = new List<StopWork>();
        [Computed]
        public string Captcha { get; set; }
        [Computed]
        public WorkStatus WorkStatus { get; set; }
        [Computed]
        public Commitment Commitment { get; set; }
        /// <summary>
        /// 非會員啟用碼
        /// </summary>
        [Computed]
        public string ActiveCode { get; set; }
    }
}
