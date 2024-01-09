using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    public class AirView : Air
    {
        [Computed]
        public string C_NO { get; set; }
        [Computed]
        public int SER_NO { get; set; }
        [Computed]
        public List<AirFile> AirFile { get; set; }
    }
}
