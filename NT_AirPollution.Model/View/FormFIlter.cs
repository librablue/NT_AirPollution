﻿using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NT_AirPollution.Model.View
{
    public class FormFilter
    {
        public string C_NO { get; set; }
        public string S_NAME { get; set; }
        public string S_G_NO { get; set; }
        public string R_NAME { get; set; }
        public string R_G_NO { get; set; }
        public string B_SERNO { get; set; }
        public string R_ADDR3 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
