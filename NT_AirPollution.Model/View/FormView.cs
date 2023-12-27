using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    public class FormView : Form
    {
        public List<Attachment> Attachments { get; set; }
    }
}
