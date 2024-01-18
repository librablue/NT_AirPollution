using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Enum
{
    /// <summary>
    /// 結算狀態
    /// </summary>
    public enum CalcStatus
    {
        未申請 = 0,
        審理中 = 1,
        需補件 = 2,
        通過待繳費 = 3,
        通過待退費小於4000 = 4,
        通過待退費大於4000 = 5,
        通過不退補 = 6,
        繳退費完成 = 7
    }
}
