using Dapper;
using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Service
{
    public class AccessService : BaseService
    {
        /// <summary>
        /// 取得最新管制編號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public string GetC_NO(FormView form)
        {
            string chineseYear = form.C_DATE.AddYears(-1911).ToString("yyy");
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault(@"
                    SELECT * FROM ABUDF
                    WHERE C_NO LIKE @C_NO+'%'
                    ORDER BY C_NO DESC",
                    new
                    {
                        C_NO = $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}"
                    });

                if (result == null)
                {
                    return $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}001";
                }

                int SerialNo = Convert.ToInt16(result.C_NO.Substring(7, 3));
                SerialNo++;
                return $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}{SerialNo.ToString().PadLeft(3, '0')}";
            }
        }

        /// <summary>
        /// 寫入ABUDF
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddABUDF(FormView form)
        {
            using (var cn = new OleDbConnection(accessConnStr))
            {
                try
                {
                    cn.Execute(@"INSERT INTO ABUDF ([C_NO],[SER_NO],[COUNTRY],[TOWN_NO],[TOWN_NA],[KIND_NO],[KIND],[AP_DATE],[B_SERNO],[PUB_COMP],[S_NAME],[S_G_NO],[S_ADDR1],[S_ADDR2],[S_TEL],[S_B_NAM],[S_B_TIT],[S_B_ID],[S_B_BDATE],[S_C_NAM],[S_C_TIT],[S_C_ID],[S_C_ADDR],[S_C_TEL],[R_NAME],[R_G_NO],[R_ADDR1],[R_ADDR2],[R_TEL],[R_B_NAM],[R_B_TIT],[R_B_ID],[R_B_BDATE],[R_ADDR3],[R_TEL1],[R_M_NAM],[R_C_NAM],[A_KIND],[AREA],[VOLUMEL],[RATIOLB],[DENSITYL],[MONEY],[C_MONEY],[PERCENT],[YEAR],[B_DATE],[E_DATE],[S_AMT],[P_KIND],[P_NUM],[P_AMT],[FIN_DATE],[FIN_COM],[STATE],[ID_DOC1],[ID_DOC2],[ID_DOC3],[COMP_DOC1],[COMP_DOC2],[COMP_DOC3],[COMP_OTH],[BUD_DOC1],[BUD_DOC2],[BUD_DOC3],[BUD_OTH],[REC_YN],[AREA_B],[AREA_F],[PERC_B],[COMP_NAM],[ADDR],[AP_TYPE],[UTME],[UTMN],[LATLNG],[AREA3],[AREA2],[O_C_Q],[G_NAME],[ADDR1],[COMP_NAM1],[ENG_STONE],[ENG_WRAP],[ENG_DES],[ENG_DES_TEL],[ENG_DES_ADDR],[NOLEVY],[COMP_L],[RCAP_DATE],[RC_DATE],[RC_SERNO],[RCE],[COMMENT],[EIACOMMENTS],[C_DATE],[M_DATE],[KEYIN],[RECCOMMENTS])
                        VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
                        new
                        {
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO,
                            COUNTRY = form.COUNTRY,
                            TOWN_NO = form.TOWN_NO,
                            TOWN_NA = form.TOWN_NA,
                            KIND_NO = form.KIND_NO,
                            KIND = form.KIND,
                            AP_DATE = form.AP_DATE,
                            B_SERNO = form.B_SERNO,
                            PUB_COMP = form.PUB_COMP,
                            S_NAME = form.S_NAME,
                            S_G_NO = form.S_G_NO,
                            S_ADDR1 = form.S_ADDR1,
                            S_ADDR2 = form.S_ADDR2,
                            S_TEL = form.S_TEL,
                            S_B_NAM = form.S_B_NAM,
                            S_B_TIT = form.S_B_TIT,
                            S_B_ID = form.S_B_ID,
                            S_B_BDATE = form.S_B_BDATE,
                            S_C_NAM = form.S_C_NAM,
                            S_C_TIT = form.S_C_TIT,
                            S_C_ID = form.S_C_ID,
                            S_C_ADDR = form.S_C_ADDR,
                            S_C_TEL = form.S_C_TEL,
                            R_NAME = form.R_NAME,
                            R_G_NO = form.R_G_NO,
                            R_ADDR1 = form.R_ADDR1,
                            R_ADDR2 = form.R_ADDR2,
                            R_TEL = form.R_TEL,
                            R_B_NAM = form.R_B_NAM,
                            R_B_TIT = form.R_B_TIT,
                            R_B_ID = form.R_B_ID,
                            R_B_BDATE = form.R_B_BDATE,
                            R_ADDR3 = form.R_ADDR3,
                            R_TEL1 = form.R_TEL1,
                            R_M_NAM = form.R_M_NAM,
                            R_C_NAM = form.R_C_NAM,
                            A_KIND = form.A_KIND,
                            AREA = form.AREA,
                            VOLUMEL = form.VOLUMEL,
                            RATIOLB = form.RATIOLB,
                            DENSITYL = form.DENSITYL,
                            MONEY = form.MONEY,
                            C_MONEY = form.C_MONEY,
                            PERCENT = form.PERCENT,
                            YEAR = form.YEAR,
                            B_DATE = form.B_DATE,
                            E_DATE = form.E_DATE,
                            S_AMT = form.S_AMT,
                            P_KIND = form.P_KIND,
                            P_NUM = form.P_NUM,
                            P_AMT = form.P_AMT,
                            FIN_DATE = form.FIN_DATE,
                            FIN_COM = form.FIN_COM,
                            STATE = form.STATE,
                            ID_DOC1 = form.ID_DOC1,
                            ID_DOC2 = form.ID_DOC2,
                            ID_DOC3 = form.ID_DOC3,
                            COMP_DOC1 = form.COMP_DOC1,
                            COMP_DOC2 = form.COMP_DOC2,
                            COMP_DOC3 = form.COMP_DOC3,
                            COMP_OTH = form.COMP_OTH,
                            BUD_DOC1 = form.BUD_DOC1,
                            BUD_DOC2 = form.BUD_DOC2,
                            BUD_DOC3 = form.BUD_DOC3,
                            BUD_OTH = form.BUD_OTH,
                            REC_YN = form.REC_YN,
                            AREA_B = form.AREA_B,
                            AREA_F = form.AREA_F,
                            PERC_B = form.PERC_B,
                            COMP_NAM = form.COMP_NAM,
                            ADDR = form.ADDR,
                            AP_TYPE = form.AP_TYPE,
                            UTME = form.UTME,
                            UTMN = form.UTMN,
                            LATLNG = form.LATLNG,
                            AREA3 = form.AREA3,
                            AREA2 = form.AREA2,
                            O_C_Q = form.O_C_Q,
                            G_NAME = form.G_NAME,
                            ADDR1 = form.ADDR1,
                            COMP_NAM1 = form.COMP_NAM1,
                            ENG_STONE = form.ENG_STONE,
                            ENG_WRAP = form.ENG_WRAP,
                            ENG_DES = form.ENG_DES,
                            ENG_DES_TEL = form.ENG_DES_TEL,
                            ENG_DES_ADDR = form.ENG_DES_ADDR,
                            NOLEVY = form.NOLEVY,
                            COMP_L = form.COMP_L,
                            RCAP_DATE = form.RCAP_DATE,
                            RC_DATE = form.RC_DATE,
                            RC_SERNO = form.RC_SERNO,
                            RCE = form.RCE,
                            COMMENT = form.COMMENT,
                            EIACOMMENTS = form.EIACOMMENTS,
                            C_DATE = form.C_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            M_DATE = form.M_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            KEYIN = form.KEYIN,
                            RECCOMMENTS = form.RECCOMMENTS
                        });

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AddABUDF: {ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }

        public bool UpdateABUDF(FormView form)
        {
            using (var cn = new OleDbConnection(accessConnStr))
            {
                try
                {
                    cn.Execute(@"UPDATE ABUDF
                                SET [TOWN_NO]=?,
                                    [TOWN_NA]=?,
                                    [KIND_NO]=?,
                                    [KIND]=?,
                                    [B_SERNO]=?,
                                    [PUB_COMP]=?,
                                    [S_NAME]=?,
                                    [S_G_NO]=?,
                                    [S_ADDR1]=?,
                                    [S_ADDR2]=?,
                                    [S_TEL]=?,
                                    [S_B_NAM]=?,
                                    [S_B_TIT]=?,
                                    [S_B_ID]=?,
                                    [S_B_BDATE]=?,
                                    [S_C_NAM]=?,
                                    [S_C_TIT]=?,
                                    [S_C_ID]=?,
                                    [S_C_ADDR]=?,
                                    [S_C_TEL]=?,
                                    [R_NAME]=?,
                                    [R_G_NO]=?,
                                    [R_ADDR1]=?,
                                    [R_ADDR2]=?,
                                    [R_TEL]=?,
                                    [R_B_NAM]=?,
                                    [R_B_TIT]=?,
                                    [R_B_ID]=?,
                                    [R_B_BDATE]=?,
                                    [R_ADDR3]=?,
                                    [R_TEL1]=?,
                                    [R_M_NAM]=?,
                                    [R_C_NAM]=?,
                                    [A_KIND]=?,
                                    [AREA]=?,
                                    [VOLUMEL]=?,
                                    [RATIOLB]=?,
                                    [DENSITYL]=?,
                                    [MONEY]=?,
                                    [C_MONEY]=?,
                                    [PERCENT]=?,
                                    [YEAR]=?,
                                    [B_DATE]=?,
                                    [E_DATE]=?,
                                    [S_AMT]=?,
                                    [P_KIND]=?,
                                    [P_NUM]=?,
                                    [P_AMT]=?,
                                    [FIN_DATE]=?,
                                    [FIN_COM]=?,
                                    [STATE]=?,
                                    [ID_DOC1]=?,
                                    [ID_DOC2]=?,
                                    [ID_DOC3]=?,
                                    [COMP_DOC1]=?,
                                    [COMP_DOC2]=?,
                                    [COMP_DOC3]=?,
                                    [COMP_OTH]=?,
                                    [BUD_DOC1]=?,
                                    [BUD_DOC2]=?,
                                    [BUD_DOC3]=?,
                                    [BUD_OTH]=?,
                                    [REC_YN]=?,
                                    [AREA_B]=?,
                                    [AREA_F]=?,
                                    [PERC_B]=?,
                                    [COMP_NAM]=?,
                                    [ADDR]=?,
                                    [AP_TYPE]=?,
                                    [UTME]=?,
                                    [UTMN]=?,
                                    [LATLNG]=?,
                                    [AREA3]=?,
                                    [AREA2]=?,
                                    [O_C_Q]=?,
                                    [G_NAME]=?,
                                    [ADDR1]=?,
                                    [COMP_NAM1]=?,
                                    [ENG_STONE]=?,
                                    [ENG_WRAP]=?,
                                    [ENG_DES]=?,
                                    [ENG_DES_TEL]=?,
                                    [ENG_DES_ADDR]=?,
                                    [NOLEVY]=?,
                                    [COMP_L]=?,
                                    [RCAP_DATE]=?,
                                    [RC_DATE]=?,
                                    [RC_SERNO]=?,
                                    [RCE]=?,
                                    [COMMENT]=?,
                                    [EIACOMMENTS]=?,
                                    [M_DATE]=?
                                WHERE [C_NO]=? AND [SER_NO]=?",
                        new
                        {
                            TOWN_NO = form.TOWN_NO,
                            TOWN_NA = form.TOWN_NA,
                            KIND_NO = form.KIND_NO,
                            KIND = form.KIND,
                            B_SERNO = form.B_SERNO,
                            PUB_COMP = form.PUB_COMP,
                            S_NAME = form.S_NAME,
                            S_G_NO = form.S_G_NO,
                            S_ADDR1 = form.S_ADDR1,
                            S_ADDR2 = form.S_ADDR2,
                            S_TEL = form.S_TEL,
                            S_B_NAM = form.S_B_NAM,
                            S_B_TIT = form.S_B_TIT,
                            S_B_ID = form.S_B_ID,
                            S_B_BDATE = form.S_B_BDATE,
                            S_C_NAM = form.S_C_NAM,
                            S_C_TIT = form.S_C_TIT,
                            S_C_ID = form.S_C_ID,
                            S_C_ADDR = form.S_C_ADDR,
                            S_C_TEL = form.S_C_TEL,
                            R_NAME = form.R_NAME,
                            R_G_NO = form.R_G_NO,
                            R_ADDR1 = form.R_ADDR1,
                            R_ADDR2 = form.R_ADDR2,
                            R_TEL = form.R_TEL,
                            R_B_NAM = form.R_B_NAM,
                            R_B_TIT = form.R_B_TIT,
                            R_B_ID = form.R_B_ID,
                            R_B_BDATE = form.R_B_BDATE,
                            R_ADDR3 = form.R_ADDR3,
                            R_TEL1 = form.R_TEL1,
                            R_M_NAM = form.R_M_NAM,
                            R_C_NAM = form.R_C_NAM,
                            A_KIND = form.A_KIND,
                            AREA = form.AREA,
                            VOLUMEL = form.VOLUMEL,
                            RATIOLB = form.RATIOLB,
                            DENSITYL = form.DENSITYL,
                            MONEY = form.MONEY,
                            C_MONEY = form.C_MONEY,
                            PERCENT = form.PERCENT,
                            YEAR = form.YEAR,
                            B_DATE = form.B_DATE,
                            E_DATE = form.E_DATE,
                            S_AMT = form.S_AMT,
                            P_KIND = form.P_KIND,
                            P_NUM = form.P_NUM,
                            P_AMT = form.P_AMT,
                            FIN_DATE = form.FIN_DATE,
                            FIN_COM = form.FIN_COM,
                            STATE = form.STATE,
                            ID_DOC1 = form.ID_DOC1,
                            ID_DOC2 = form.ID_DOC2,
                            ID_DOC3 = form.ID_DOC3,
                            COMP_DOC1 = form.COMP_DOC1,
                            COMP_DOC2 = form.COMP_DOC2,
                            COMP_DOC3 = form.COMP_DOC3,
                            COMP_OTH = form.COMP_OTH,
                            BUD_DOC1 = form.BUD_DOC1,
                            BUD_DOC2 = form.BUD_DOC2,
                            BUD_DOC3 = form.BUD_DOC3,
                            BUD_OTH = form.BUD_OTH,
                            REC_YN = form.REC_YN,
                            AREA_B = form.AREA_B,
                            AREA_F = form.AREA_F,
                            PERC_B = form.PERC_B,
                            COMP_NAM = form.COMP_NAM,
                            ADDR = form.ADDR,
                            AP_TYPE = form.AP_TYPE,
                            UTME = form.UTME,
                            UTMN = form.UTMN,
                            LATLNG = form.LATLNG,
                            AREA3 = form.AREA3,
                            AREA2 = form.AREA2,
                            O_C_Q = form.O_C_Q,
                            G_NAME = form.G_NAME,
                            ADDR1 = form.ADDR1,
                            COMP_NAM1 = form.COMP_NAM1,
                            ENG_STONE = form.ENG_STONE,
                            ENG_WRAP = form.ENG_WRAP,
                            ENG_DES = form.ENG_DES,
                            ENG_DES_TEL = form.ENG_DES_TEL,
                            ENG_DES_ADDR = form.ENG_DES_ADDR,
                            NOLEVY = form.NOLEVY,
                            COMP_L = form.COMP_L,
                            RCAP_DATE = form.RCAP_DATE,
                            RC_DATE = form.RC_DATE,
                            RC_SERNO = form.RC_SERNO,
                            RCE = form.RCE,
                            COMMENT = form.COMMENT,
                            EIACOMMENTS = form.EIACOMMENTS,
                            M_DATE = form.M_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO
                        });

                    return true;
                }
                catch (Exception ex)
                {
                    Logger.Error($"UpdateABUDF: {ex.Message}");
                    throw new Exception("系統發生未預期錯誤");
                }
            }
        }
    }
}
