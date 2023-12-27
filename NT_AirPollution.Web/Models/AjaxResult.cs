using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NT_AirPollution.Web.Models
{
    public class AjaxResult
    {
        public bool Status { get; set; }
        public object Message { get; set; }
    }
}