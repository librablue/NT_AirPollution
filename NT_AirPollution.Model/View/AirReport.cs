﻿using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    public class AirReport : Form
    {
        public long AirID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Position { get; set; }
        public string Method { get; set; }
        public string Remark { get; set; }
        public List<AirFile> AirFiles { get; set; } = new List<AirFile>();
    }
}
