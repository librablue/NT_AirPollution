using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NT_AirPollution.Model.View
{
    public class AttachmentFile
    {
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        public HttpPostedFileBase File1 { get; set; }
        public HttpPostedFileBase File2 { get; set; }
        public HttpPostedFileBase File3 { get; set; }
        public HttpPostedFileBase File4 { get; set; }
        public HttpPostedFileBase File5 { get; set; }
        public HttpPostedFileBase File6 { get; set; }
        public HttpPostedFileBase File7 { get; set; }
        public HttpPostedFileBase File8 { get; set; }
    }
}
