using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    [Table("RoadPromise")]
    public class RoadPromiseView : RoadPromise
    {
        /// <summary>
        /// 管制編號
        /// </summary>
        [Computed]
        public string C_NO { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        [Computed]
        public int? SER_NO { get; set; }

        /// <summary>
        /// 工程名稱
        /// </summary>
        [Computed]
        public string COMP_NAM { get; set; }

        /// <summary>
        /// 工地地址或地號
        /// </summary>
        [Computed]
        public string ADDR { get; set; }

        /// <summary>
        /// 工地主任姓名
        /// </summary>
        [Computed]
        public string R_M_NAM { get; set; }

        /// <summary>
        /// 承包商電話
        /// </summary>
        [Computed]
        public string R_TEL { get; set; }

        /// <summary>
        /// 工務所電話
        /// </summary>
        [Computed]
        public string R_TEL1 { get; set; }

        [Computed]
        public List<Road> Roads { get; set; } = new List<Road>();
    }
}
