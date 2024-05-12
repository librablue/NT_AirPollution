using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NT_AirPollution.Admin.Models
{
    public class UploadResponse
    {
        public int uploaded { get; set; }
        public string url { get; set; }
        public string filename { get; set; }
    }
}