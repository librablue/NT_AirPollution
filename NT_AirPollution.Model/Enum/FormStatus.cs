using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Enum
{
    /// <summary>
    /// 申請單狀態
    /// </summary>
    public enum FormStatus
    {
        未申請 = 0,
        審理中 = 1,
        待補件 = 2,
        通過待繳費 = 3,
        已繳費完成 = 4
    }
}
