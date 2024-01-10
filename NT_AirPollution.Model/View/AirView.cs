using Dapper.Contrib.Extensions;
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
        //[Computed]
        //public string C_NO { get; set; }
        //[Computed]
        //public int SER_NO { get; set; }
        //[Computed]
        //public string COMP_NAM { get; set; }
        //[Computed]
        //public string B_DATE { get; set; }
        //[Computed]
        //public string B_DATE2 { get; set; }
        //[Computed]
        //public string E_DATE { get; set; }
        //[Computed]
        //public string E_DATE2 { get; set; }
        [Computed]
        public List<AirFile> AirFiles { get; set; } = new List<AirFile>();
    }
}
