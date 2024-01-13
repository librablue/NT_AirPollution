using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace NT_AirPollution.Model.View
{
    public class RoadExcel
    {
        public long PromiseID { get; set; }
        public long RoadID { get; set; }
        public string RoadName { get; set; }
        public string CleanWay1 { get; set; }
        public string CleanWay2 { get; set; }
        /// <summary>
        /// 認養期程(起)
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 認養期程(迄)
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 承諾書簽署日
        /// </summary>
        public DateTime PromiseCreateDate { get; set; }
        public double RoadLength { get; set; }
        public string C_NO { get; set; }
        public int SER_NO { get; set; }
        public string TOWN_NA { get; set; }
        public string KIND { get; set; }
        public DateTime C_DATE { get; set; }
        public string S_NAME { get; set; }
        public List<RoadExcelMonth> RoadExcelMonth { get; set; } = new List<RoadExcelMonth>();
    }

    public class RoadExcelMonth
    {
        public int YearMth { get; set; }
        /// <summary>
        /// 洗街長度
        /// </summary>
        public int CleanLength1 { get; set; }
        /// <summary>
        /// 掃街長度
        /// </summary>
        public int CleanLength2 { get; set; }

    }
}
