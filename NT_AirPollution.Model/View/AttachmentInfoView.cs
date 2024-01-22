using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NT_AirPollution.Model.View
{
    public class AttachmentInfoView: AttachmentInfo
    {
        public string FileName { get; set; }
    }
}
