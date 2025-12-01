using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Access
{
    public class ABUDF_B
    {
        /// <summary>
        /// 管制編號
        /// </summary>
        public string C_NO { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int SER_NO { get; set; }

        /// <summary>
        /// 結算申請日期
        /// </summary>
        public string AP_DATE1 { get; set; }

        /// <summary>
        /// 結算狀態
        /// </summary>
        public string B_STAT { get; set; }

        /// <summary>
        /// 施工狀態
        /// </summary>
        public string B_CSTAT { get; set; }

        /// <summary>
        /// 工程類別代碼
        /// </summary>
        public string KIND_NO { get; set; }

        /// <summary>
        /// 工程類別
        /// </summary>
        public string KIND { get; set; }

        /// <summary>
        /// 費率年度
        /// </summary>
        public string YEAR { get; set; }

        /// <summary>
        /// 工程費基種類
        /// </summary>
        public string A_KIND { get; set; }

        /// <summary>
        /// 合約經費
        /// </summary>
        public double MONEY { get; set; }

        /// <summary>
        /// 工程面積
        /// </summary>
        public double AREA { get; set; }

        /// <summary>
        /// 外運土石鬆方
        /// </summary>
        public double VOLUMEL { get; set; }

        /// <summary>
        /// 鬆實方體積比值
        /// </summary>
        public double RATIOLB { get; set; }

        /// <summary>
        /// 鬆方密度值
        /// </summary>
        public double DENSITYL { get; set; }

        /// <summary>
        /// 核定扣除日數
        /// </summary>
        public int B_DAY { get; set; }

        /// <summary>
        /// 実際施工期程(起日)
        /// </summary>
        public string B_DATE { get; set; }

        /// <summary>
        /// 實際施工期程(迄日)
        /// </summary>
        public string E_DATE { get; set; }

        /// <summary>
        /// 實際施工年數
        /// </summary>
        public double B_YEAR { get; set; }

        /// <summary>
        /// 空污費應繳總金額
        /// </summary>
        public double S_AMT { get; set; }

        /// <summary>
        /// 核算減免總金額
        /// </summary>
        public double D_AMT { get; set; }

        /// <summary>
        /// 工程種類異動
        /// </summary>
        public string CHG_KIND { get; set; }

        /// <summary>
        /// 費率年度異動
        /// </summary>
        public string CHG_YEAR { get; set; }

        /// <summary>
        /// 合約經費異動
        /// </summary>
        public string CHG_MONEY { get; set; }

        /// <summary>
        /// 工程面積異動
        /// </summary>
        public string CHG_AREA { get; set; }

        /// <summary>
        /// 外運土石鬆方異動
        /// </summary>
        public string CHG_VOLUMEL { get; set; }

        /// <summary>
        /// 施工期程異動
        /// </summary>
        public string CHG_DATE { get; set; }

        /// <summary>
        /// 停工異動
        /// </summary>
        public string CHG_DAY { get; set; }

        /// <summary>
        /// 綜合異動項目
        /// </summary>
        public string CHG_COMM { get; set; }

        /// <summary>
        /// 應繳金額異動
        /// </summary>
        public double CHG_AMT { get; set; }

        /// <summary>
        /// 工期扣除認定證明文件
        /// </summary>
        public string B_KIND1 { get; set; }

        /// <summary>
        /// 污染防制設備設置或操作記錄
        /// </summary>
        public string B_KIND2 { get; set; }

        /// <summary>
        /// 有無業主負責人身分證影本
        /// </summary>
        public string ID_DOC1 { get; set; }

        /// <summary>
        /// 有無業主聯絡人身分證影本
        /// </summary>
        public string ID_DOC2 { get; set; }

        /// <summary>
        /// 有無業主承包商身分證影本
        /// </summary>
        public string ID_DOC3 { get; set; }

        /// <summary>
        /// 有無公司執照
        /// </summary>
        public string COMP_DOC1 { get; set; }

        /// <summary>
        /// 有無營利事業登記證
        /// </summary>
        public string COMP_DOC2 { get; set; }

        /// <summary>
        /// 有無其他證書
        /// </summary>
        public string COMP_DOC3 { get; set; }

        /// <summary>
        /// 其他證書
        /// </summary>
        public string COMP_OTH { get; set; }

        /// <summary>
        /// 有無建築執照
        /// </summary>
        public string BUD_DOC1 { get; set; }

        /// <summary>
        /// 有無施工計畫書
        /// </summary>
        public string BUD_DOC2 { get; set; }

        /// <summary>
        /// 有無其他工程資料
        /// </summary>
        public string BUD_DOC3 { get; set; }

        /// <summary>
        /// 其他工程資料
        /// </summary>
        public string BUD_OTH { get; set; }

        /// <summary>
        /// 實際施工日數
        /// </summary>
        public int T_DAY { get; set; }

        /// <summary>
        /// 建築面積
        /// </summary>
        public double AREA_B { get; set; }

        /// <summary>
        /// 基地面積
        /// </summary>
        public double AREA_F { get; set; }

        /// <summary>
        /// 建蔽率
        /// </summary>
        public double PERC_B { get; set; }

        /// <summary>
        /// 尚須退費金額
        /// </summary>
        public long PRE_C_AMT { get; set; }

        /// <summary>
        /// 尚須補繳金額
        /// </summary>
        public long PRE_C_AMT1 { get; set; }

        /// <summary>
        /// 未退費原因
        /// </summary>
        public string REASON { get; set; }

        /// <summary>
        /// 未補繳原因
        /// </summary>
        public string REASON1 { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string OPINION { get; set; }

        /// <summary>
        /// 是否漏報短報
        /// </summary>
        public string WRONG_AP { get; set; }

        /// <summary>
        /// 資料異動職工編號
        /// </summary>
        public string KEYIN { get; set; }

        /// <summary>
        /// 建檔日期
        /// </summary>
        public DateTime C_DATE { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime M_DATE { get; set; }
    }
}
