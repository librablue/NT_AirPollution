﻿using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    [Table("Air")]
    public class AirView : Air
    {
        [Computed]
        public List<AirFile> AirFiles { get; set; } = new List<AirFile>();
    }
}
