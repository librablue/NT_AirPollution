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
        public Attachment Attachment { get; set; } = new Attachment();
        [Computed]
        public List<StopWork> StopWorks { get; set; } = new List<StopWork>();
        [Computed]
        public string Captcha { get; set; }
        [Computed]
        public WorkStatus WorkStatus { get; set; }
        [Computed]
        public Commitment Commitment { get; set; }
    }
}
