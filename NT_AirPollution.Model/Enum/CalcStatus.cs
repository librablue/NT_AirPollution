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
        通過待繳費 = 30,
        通過待退費 = 31,
        已繳費完成 = 40,
        已退費完成 = 41
    }
}
