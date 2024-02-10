using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Form")]
    public class Form
    {
        [Dapper.Contrib.Extensions.Key]
        public long ID { get; set; }

        /// <summary>
        /// 管制編號
        /// </summary>
        public string C_NO { get; set; }

        /// <summary>
        /// 序號
        /// </summary>
        public int? SER_NO { get; set; }

        /// <summary>
        /// 縣市代碼
        /// </summary>
        public string COUNTRY { get; set; } = "M";

        /// <summary>
        /// 鄉鎮代碼
        /// </summary>
        [Required(ErrorMessage = "請選擇鄉鎮代碼")]
        public string TOWN_NO { get; set; }

        /// <summary>
        /// 鄉鎮名稱
        /// </summary>
        public string TOWN_NA { get; set; }

        /// <summary>
        /// 工程類別代碼
        /// </summary>
        public string KIND_NO { get; set; }

        /// <summary>
        /// 工程類別
        /// </summary>
        public string KIND { get; set; }

        /// <summary>
        /// 申報日期
        /// </summary>
        public string AP_DATE { get; set; }

        /// <summary>
        /// 結算日期
        /// </summary>
        public string AP_DATE1 { get; set; }

        /// <summary>
        /// 建照字號或合約編號
        /// </summary>
        [Required(ErrorMessage = "請輸入建照字號或合約編號")]
        public string B_SERNO { get; set; }

        /// <summary>
        /// 是否公共工程
        /// </summary>
        [Required(ErrorMessage = "請選擇是否公共工程")]
        public bool PUB_COMP { get; set; }

        /// <summary>
        /// 營建業主名稱
        /// </summary>
        [Required(ErrorMessage = "請輸入營建業主名稱")]
        public string S_NAME { get; set; }

        /// <summary>
        /// 營建業主營利事業統一編號
        /// </summary>
        public string S_G_NO { get; set; }

        /// <summary>
        /// 業主營業地址
        /// </summary>
        public string S_ADDR1 { get; set; }

        /// <summary>
        /// 業主聯絡地址
        /// </summary>
        public string S_ADDR2 { get; set; }

        /// <summary>
        /// 業主電話
        /// </summary>
        public string S_TEL { get; set; }

        /// <summary>
        /// 業主負責人
        /// </summary>
        public string S_B_NAM { get; set; }

        /// <summary>
        /// 業主負責人職稱
        /// </summary>
        public string S_B_TIT { get; set; }

        /// <summary>
        /// 業主負責人身分證字號
        /// </summary>
        public string S_B_ID { get; set; }

        /// <summary>
        /// 業主負責人生日
        /// </summary>
        public string S_B_BDATE { get; set; }
        [Computed]
        public DateTime S_B_BDATE2 { get; set; }

        /// <summary>
        /// 業主聯絡人
        /// </summary>
        public string S_C_NAM { get; set; }

        /// <summary>
        /// 業主聯絡人職稱
        /// </summary>
        public string S_C_TIT { get; set; }

        /// <summary>
        /// 業主聯絡人身分證字號
        /// </summary>
        public string S_C_ID { get; set; }

        /// <summary>
        /// 業主聯絡人地址
        /// </summary>
        public string S_C_ADDR { get; set; }

        /// <summary>
        /// 業主聯絡人電話
        /// </summary>
        public string S_C_TEL { get; set; }

        /// <summary>
        /// 承包商名稱
        /// </summary>
        public string R_NAME { get; set; }

        /// <summary>
        /// 承包商營利事業統一編號
        /// </summary>
        public string R_G_NO { get; set; }

        /// <summary>
        /// 承包商營業地址
        /// </summary>
        public string R_ADDR1 { get; set; }

        /// <summary>
        /// 承包商聯絡地址
        /// </summary>
        public string R_ADDR2 { get; set; }

        /// <summary>
        /// 承包商電話
        /// </summary>
        public string R_TEL { get; set; }

        /// <summary>
        /// 承包商負責人
        /// </summary>
        public string R_B_NAM { get; set; }

        /// <summary>
        /// 承包商負責人職稱
        /// </summary>
        public string R_B_TIT { get; set; }

        /// <summary>
        /// 承包商負責人身分證字號
        /// </summary>
        public string R_B_ID { get; set; }

        /// <summary>
        /// 承包商負責人生日
        /// </summary>
        public string R_B_BDATE { get; set; }
        [Computed]
        public DateTime R_B_BDATE2 { get; set; }

        /// <summary>
        /// 工務所地址
        /// </summary>
        public string R_ADDR3 { get; set; }

        /// <summary>
        /// 工務所電話
        /// </summary>
        public string R_TEL1 { get; set; }

        /// <summary>
        /// 工地主任姓名
        /// </summary>
        public string R_M_NAM { get; set; }

        /// <summary>
        /// 工地環保負責人姓名
        /// </summary>
        public string R_C_NAM { get; set; }

        /// <summary>
        /// 工程費基種類
        /// </summary>
        public string A_KIND { get; set; }

        /// <summary>
        /// 工程面積
        /// </summary>
        public double? AREA { get; set; }

        /// <summary>
        /// 外運土石鬆方
        /// </summary>
        public double? VOLUMEL { get; set; }

        /// <summary>
        /// 鬆實方體積比值
        /// </summary>
        public double? RATIOLB { get; set; }

        /// <summary>
        /// 鬆方密度值
        /// </summary>
        public double? DENSITYL { get; set; }

        /// <summary>
        /// 合約經費
        /// </summary>
        public double MONEY { get; set; }

        /// <summary>
        /// 工程環保經費
        /// </summary>
        public double? C_MONEY { get; set; }

        /// <summary>
        /// 環保經費佔總工程經費之比例％
        /// </summary>
        public double? PERCENT { get; set; }

        /// <summary>
        /// 費率年度
        /// </summary>
        public string YEAR { get; set; } = "103";

        /// <summary>
        /// 預計施工期程(起日)
        /// </summary>
        public string B_DATE { get; set; }
        [Computed]
        public DateTime B_DATE2 { get; set; }

        /// <summary>
        /// 預計施工期程(迄日)
        /// </summary>
        public string E_DATE { get; set; }
        [Computed]
        public DateTime E_DATE2 { get; set; }

        /// <summary>
        /// 應繳總金額
        /// </summary>
        public double? S_AMT { get; set; }

        /// <summary>
        /// 結算應繳金額
        /// </summary>
        public double? S_AMT2 { get; set; }

        /// <summary>
        /// 繳款方式
        /// </summary>
        public string P_KIND { get; set; }

        /// <summary>
        /// 期數
        /// </summary>
        public int P_NUM { get; set; }

        /// <summary>
        /// 每次應繳金額
        /// </summary>
        public double? P_AMT { get; set; }

        /// <summary>
        /// 已完成結算繳費程序日期
        /// </summary>
        public string FIN_DATE { get; set; }

        /// <summary>
        /// 已完成完工查核日期
        /// </summary>
        public string FIN_COM { get; set; }

        /// <summary>
        /// 工程內容概述
        /// </summary>
        public string STATE { get; set; }

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
        /// 是否繳交空氣污染防制措施承諾書(有,無)
        /// </summary>
        public string REC_YN { get; set; }

        /// <summary>
        /// 建築面積
        /// </summary>
        public double? AREA_B { get; set; }

        /// <summary>
        /// 基地面積
        /// </summary>
        public double? AREA_F { get; set; }

        /// <summary>
        /// 建蔽率
        /// </summary>
        public double? PERC_B { get; set; }

        /// <summary>
        /// 工程名稱
        /// </summary>
        public string COMP_NAM { get; set; }

        /// <summary>
        /// 工地地址或地號
        /// </summary>
        public string ADDR { get; set; }

        /// <summary>
        /// 申報方式
        /// </summary>
        public string AP_TYPE { get; set; }

        /// <summary>
        /// UTM 座標_X
        /// </summary>
        public double? UTME { get; set; }

        /// <summary>
        /// UTM 座標_Y
        /// </summary>
        public double? UTMN { get; set; }

        /// <summary>
        /// 座標( 緯度, 經度) 多筆分號(;)分隔
        /// </summary>
        public string LATLNG { get; set; }

        /// <summary>
        /// 管線道路隧道長度
        /// </summary>
        public double? AREA3 { get; set; }

        /// <summary>
        /// 總樓地板面積
        /// </summary>
        public double? AREA2 { get; set; }

        /// <summary>
        /// 工地外車流量
        /// </summary>
        public double? O_C_Q { get; set; }

        /// <summary>
        /// 棄土場名稱
        /// </summary>
        public string G_NAME { get; set; }

        /// <summary>
        /// 工地地址
        /// </summary>
        public string ADDR1 { get; set; }

        /// <summary>
        /// 實際工程名稱
        /// </summary>
        public string COMP_NAM1 { get; set; }

        /// <summary>
        /// 土方或砂石用量
        /// </summary>
        public double? ENG_STONE { get; set; }

        /// <summary>
        /// 土方開挖
        /// </summary>
        public double? ENG_WRAP { get; set; }

        /// <summary>
        /// 監造單位
        /// </summary>
        public string ENG_DES { get; set; }

        /// <summary>
        /// 監造單位聯絡電話
        /// </summary>
        public string ENG_DES_TEL { get; set; }

        /// <summary>
        /// 監造單位地址
        /// </summary>
        public string ENG_DES_ADDR { get; set; }

        /// <summary>
        /// 免徵原因說明
        /// </summary>
        public string NOLEVY { get; set; }

        /// <summary>
        /// 管理辦法工程等級
        /// </summary>
        public string COMP_L { get; set; }

        /// <summary>
        /// 替代防制設施申請日期
        /// </summary>
        public string RCAP_DATE { get; set; }

        /// <summary>
        /// 替代防制設施核可日期
        /// </summary>
        public string RC_DATE { get; set; }

        /// <summary>
        /// 替代防制設施核可文號
        /// </summary>
        public string RC_SERNO { get; set; }

        /// <summary>
        /// 替代防制設施申請原因
        /// </summary>
        public string RCE { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string COMMENT { get; set; }

        /// <summary>
        /// 環評保護對策
        /// </summary>
        public string EIACOMMENTS { get; set; }

        /// <summary>
        /// 資料首次輸入日期
        /// </summary>
        public DateTime? C_DATE { get; set; }

        /// <summary>
        /// 資料修改輸入日期
        /// </summary>
        public DateTime? M_DATE { get; set; }

        /// <summary>
        /// 資料異動職工編號
        /// </summary>
        public string KEYIN { get; set; }

        /// <summary>
        /// 記錄備註
        /// </summary>
        public string RECCOMMENTS { get; set; }

        /// <summary>
        /// 會員ID
        /// </summary>
        public long? ClientUserID { get; set; }

        /// <summary>
        /// 申請人Email(非會員用)
        /// </summary>
        public string CreateUserEmail { get; set; }
        /// <summary>
        /// 申請人姓名
        /// </summary>
        public string CreateUserName { get; set; }
        /// <summary>
        /// 非會員啟用碼
        /// </summary>
        public string ActiveCode { get; set; }
        /// <summary>
        /// 是否啟用
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// 申請單狀態
        /// </summary>
        public FormStatus FormStatus { get; set; }
        /// <summary>
        /// 審核日期
        /// </summary>
        public DateTime? VerifyDate1 { get; set; }
        /// <summary>
        /// 退件原因
        /// </summary>
        public string FailReason1 { get; set; }
        /// <summary>
        /// 結算狀態
        /// </summary>
        public CalcStatus CalcStatus { get; set; }
        /// <summary>
        /// 結算審核日期
        /// </summary>
        public DateTime? VerifyDate2 { get; set; }
        /// <summary>
        /// 結算退件原因
        /// </summary>
        public string FailReason2 { get; set; }
    }
}
