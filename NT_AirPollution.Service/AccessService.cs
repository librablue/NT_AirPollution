using Dapper;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service.Extensions;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;

namespace NT_AirPollution.Service
{
    public class AccessService : BaseService
    {
        private readonly string domain = "192.168.96.16";
        private readonly string userName = "cpc";
        private readonly string password = "Ciohe@2565!";

        /// <summary>
        /// 取得最新管制編號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public string GetC_NO(FormView form)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                string chineseYear = form.C_DATE.Value.AddYears(-1911).ToString("yyy");
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    var result = cn.QueryFirstOrDefault(@"
                        SELECT * FROM ABUDF
                        WHERE C_NO LIKE @C_NO+'%'
                        ORDER BY C_NO DESC",
                        new
                        {
                            C_NO = $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}"
                        }, commandTimeout: 180);

                    if (result == null)
                    {
                        return $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}001";
                    }

                    int SerialNo = Convert.ToInt16(result.C_NO.Substring(7, 3));
                    SerialNo++;
                    return $"M{chineseYear}{form.TOWN_NO}{form.KIND_NO}{SerialNo.ToString().PadLeft(3, '0')}";
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 取得管制編號最大序號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public int GetMaxSER_NOByC_NO(FormView form)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    var result = cn.QueryFirstOrDefault(@"
                        SELECT SER_NO FROM ABUDF
                        WHERE C_NO=@C_NO
                        ORDER BY SER_NO DESC",
                        new { C_NO = form.C_NO }, commandTimeout: 180);

                    return result?.SER_NO ?? 1;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
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
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"
                        INSERT INTO ABUDF (
                            [C_NO],[SER_NO],[COUNTRY],[TOWN_NO],[TOWN_NA],[KIND_NO],[KIND],[AP_DATE],[B_SERNO],[PUB_COMP],
                            [S_NAME],[S_G_NO],[S_ADDR1],[S_ADDR2],[S_TEL],[S_B_NAM],[S_B_TIT],[S_B_ID],[S_B_BDATE],[S_C_NAM],
                            [S_C_TIT],[S_C_ID],[S_C_ADDR],[S_C_TEL],[R_NAME],[R_G_NO],[R_ADDR1],[R_ADDR2],[R_TEL],
                            [R_B_NAM],[R_B_TIT],[R_B_ID],[R_B_BDATE],[R_ADDR3],[R_TEL1],[R_M_NAM],[R_C_NAM],[A_KIND],
                            [AREA],[VOLUMEL],[RATIOLB],[DENSITYL],[MONEY],[C_MONEY],[PERCENT],[YEAR],[B_DATE],[E_DATE],
                            [S_AMT],[P_KIND],[P_NUM],[P_AMT],[FIN_DATE],[FIN_COM],[STATE],[ID_DOC1],[ID_DOC2],[ID_DOC3],
                            [COMP_DOC1],[COMP_DOC2],[COMP_DOC3],[COMP_OTH],[BUD_DOC1],[BUD_DOC2],[BUD_DOC3],[BUD_OTH],
                            [REC_YN],[AREA_B],[AREA_F],[PERC_B],[COMP_NAM],[ADDR],[AP_TYPE],[UTME],[UTMN],[LATLNG],
                            [AREA3],[AREA2],[O_C_Q],[G_NAME],[ADDR1],[COMP_NAM1],[ENG_STONE],[ENG_WRAP],[ENG_DES],
                            [ENG_DES_TEL],[ENG_DES_ADDR],[NOLEVY],[COMP_L],[RCAP_DATE],[RC_DATE],[RC_SERNO],[RCE],
                            [COMMENT],[EIACOMMENTS],[C_DATE],[M_DATE],[KEYIN],[RECCOMMENTS]
                        )
                        VALUES (
                            @C_NO,@SER_NO,@COUNTRY,@TOWN_NO,@TOWN_NA,@KIND_NO,@KIND,@AP_DATE,@B_SERNO,@PUB_COMP,
                            @S_NAME,@S_G_NO,@S_ADDR1,@S_ADDR2,@S_TEL,@S_B_NAM,@S_B_TIT,@S_B_ID,@S_B_BDATE,@S_C_NAM,
                            @S_C_TIT,@S_C_ID,@S_C_ADDR,@S_C_TEL,@R_NAME,@R_G_NO,@R_ADDR1,@R_ADDR2,@R_TEL,
                            @R_B_NAM,@R_B_TIT,@R_B_ID,@R_B_BDATE,@R_ADDR3,@R_TEL1,@R_M_NAM,@R_C_NAM,@A_KIND,
                            @AREA,@VOLUMEL,@RATIOLB,@DENSITYL,@MONEY,@C_MONEY,@PERCENT,@YEAR,@B_DATE,@E_DATE,
                            @S_AMT,@P_KIND,@P_NUM,@P_AMT,@FIN_DATE,@FIN_COM,@STATE,@ID_DOC1,@ID_DOC2,@ID_DOC3,
                            @COMP_DOC1,@COMP_DOC2,@COMP_DOC3,@COMP_OTH,@BUD_DOC1,@BUD_DOC2,@BUD_DOC3,@BUD_OTH,
                            @REC_YN,@AREA_B,@AREA_F,@PERC_B,@COMP_NAM,@ADDR,@AP_TYPE,@UTME,@UTMN,@LATLNG,
                            @AREA3,@AREA2,@O_C_Q,@G_NAME,@ADDR1,@COMP_NAM1,@ENG_STONE,@ENG_WRAP,@ENG_DES,
                            @ENG_DES_TEL,@ENG_DES_ADDR,@NOLEVY,@COMP_L,@RCAP_DATE,@RC_DATE,@RC_SERNO,@RCE,
                            @COMMENT,@EIACOMMENTS,@C_DATE,@M_DATE,@KEYIN,@RECCOMMENTS
                        )",
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
                            MONEY = form.MONEY - form.TAX_MONEY,
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
                            UTME = Math.Round(form.UTME ?? 0, 0),
                            UTMN = Math.Round(form.UTMN ?? 0, 0),
                            LATLNG = form.LATLNG,
                            AREA3 = form.AREA3,
                            AREA2 = form.AREA2,
                            O_C_Q = form.O_C_Q,
                            G_NAME = form.G_NAME,
                            ADDR1 = form.ADDR1,
                            COMP_NAM1 = form.COMP_NAM1,
                            ENG_STONE = form.ENG_STONE,
                            ENG_WRAP = form.ENG_WRAP ?? 0,
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
                            C_DATE = form.C_DATE?.ToString("yyyy-MM-dd HH:mm:ss"),
                            M_DATE = form.M_DATE?.ToString("yyyy-MM-dd HH:mm:ss"),
                            KEYIN = "EPB02",
                            RECCOMMENTS = form.RECCOMMENTS
                        },
                        commandTimeout: 180
                    );

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 取得單筆ABUDF
        /// </summary>
        /// <param name="c_no">管制編號</param>
        /// <param name="bdate">序號</param>
        /// <returns></returns>
        public ABUDF GetABUDF(string c_no, int ser_no)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault<ABUDF>(@"
                    SELECT * FROM ABUDF WHERE C_NO=@C_NO AND SER_NO=@SER_NO",
                    new { C_NO = c_no, SER_NO = ser_no });

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 取得單筆ABUDF
        /// </summary>
        /// <param name="c_no">管制編號</param>
        /// <param name="bdate">開工日</param>
        /// <returns></returns>
        public ABUDF GetABUDF(string c_no, string bdate)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault<ABUDF>(@"
                    SELECT * FROM ABUDF WHERE C_NO=@C_NO AND B_DATE=@B_DATE",
                    new { C_NO = c_no, B_DATE = bdate });

                return result;
            }
#if !DEBUG
            }
#endif
        }

        public bool UpdateABUDF(FormView form)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"
                        UPDATE ABUDF SET
                            [TOWN_NO]=@TOWN_NO,
                            [TOWN_NA]=@TOWN_NA,
                            [KIND_NO]=@KIND_NO,
                            [KIND]=@KIND,
                            [B_SERNO]=@B_SERNO,
                            [PUB_COMP]=@PUB_COMP,
                            [S_NAME]=@S_NAME,
                            [S_G_NO]=@S_G_NO,
                            [S_ADDR1]=@S_ADDR1,
                            [S_ADDR2]=@S_ADDR2,
                            [S_TEL]=@S_TEL,
                            [S_B_NAM]=@S_B_NAM,
                            [S_B_TIT]=@S_B_TIT,
                            [S_B_ID]=@S_B_ID,
                            [S_B_BDATE]=@S_B_BDATE,
                            [S_C_NAM]=@S_C_NAM,
                            [S_C_TIT]=@S_C_TIT,
                            [S_C_ID]=@S_C_ID,
                            [S_C_ADDR]=@S_C_ADDR,
                            [S_C_TEL]=@S_C_TEL,
                            [R_NAME]=@R_NAME,
                            [R_G_NO]=@R_G_NO,
                            [R_ADDR1]=@R_ADDR1,
                            [R_ADDR2]=@R_ADDR2,
                            [R_TEL]=@R_TEL,
                            [R_B_NAM]=@R_B_NAM,
                            [R_B_TIT]=@R_B_TIT,
                            [R_B_ID]=@R_B_ID,
                            [R_B_BDATE]=@R_B_BDATE,
                            [R_ADDR3]=@R_ADDR3,
                            [R_TEL1]=@R_TEL1,
                            [R_M_NAM]=@R_M_NAM,
                            [R_C_NAM]=@R_C_NAM,
                            [A_KIND]=@A_KIND,
                            [AREA]=@AREA,
                            [VOLUMEL]=@VOLUMEL,
                            [RATIOLB]=@RATIOLB,
                            [DENSITYL]=@DENSITYL,
                            [MONEY]=@MONEY,
                            [C_MONEY]=@C_MONEY,
                            [PERCENT]=@PERCENT,
                            [YEAR]=@YEAR,
                            [B_DATE]=@B_DATE,
                            [E_DATE]=@E_DATE,
                            [S_AMT]=@S_AMT,
                            [P_KIND]=@P_KIND,
                            [P_NUM]=@P_NUM,
                            [P_AMT]=@P_AMT,
                            [FIN_DATE]=@FIN_DATE,
                            [FIN_COM]=@FIN_COM,
                            [STATE]=@STATE,
                            [ID_DOC1]=@ID_DOC1,
                            [ID_DOC2]=@ID_DOC2,
                            [ID_DOC3]=@ID_DOC3,
                            [COMP_DOC1]=@COMP_DOC1,
                            [COMP_DOC2]=@COMP_DOC2,
                            [COMP_DOC3]=@COMP_DOC3,
                            [COMP_OTH]=@COMP_OTH,
                            [BUD_DOC1]=@BUD_DOC1,
                            [BUD_DOC2]=@BUD_DOC2,
                            [BUD_DOC3]=@BUD_DOC3,
                            [BUD_OTH]=@BUD_OTH,
                            [REC_YN]=@REC_YN,
                            [AREA_B]=@AREA_B,
                            [AREA_F]=@AREA_F,
                            [PERC_B]=@PERC_B,
                            [COMP_NAM]=@COMP_NAM,
                            [ADDR]=@ADDR,
                            [AP_TYPE]=@AP_TYPE,
                            [UTME]=@UTME,
                            [UTMN]=@UTMN,
                            [LATLNG]=@LATLNG,
                            [AREA3]=@AREA3,
                            [AREA2]=@AREA2,
                            [O_C_Q]=@O_C_Q,
                            [G_NAME]=@G_NAME,
                            [ADDR1]=@ADDR1,
                            [COMP_NAM1]=@COMP_NAM1,
                            [ENG_STONE]=@ENG_STONE,
                            [ENG_WRAP]=@ENG_WRAP,
                            [ENG_DES]=@ENG_DES,
                            [ENG_DES_TEL]=@ENG_DES_TEL,
                            [ENG_DES_ADDR]=@ENG_DES_ADDR,
                            [NOLEVY]=@NOLEVY,
                            [COMP_L]=@COMP_L,
                            [RCAP_DATE]=@RCAP_DATE,
                            [RC_DATE]=@RC_DATE,
                            [RC_SERNO]=@RC_SERNO,
                            [RCE]=@RCE,
                            [COMMENT]=@COMMENT,
                            [EIACOMMENTS]=@EIACOMMENTS,
                            [M_DATE]=@M_DATE
                        WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO",
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
                            VOLUMEL = form.VOLUMEL ?? 0,
                            RATIOLB = form.RATIOLB,
                            DENSITYL = form.DENSITYL,
                            MONEY = form.MONEY - form.TAX_MONEY,
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
                            ENG_WRAP = form.ENG_WRAP ?? 0,
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
                            M_DATE = form.M_DATE?.ToString("yyyy-MM-dd HH:mm:ss"),
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO
                        },
                        commandTimeout: 180);

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 更新ABUDF單一欄位
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <param name="key">欄位名稱</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool UpdateABUDFByColumn(string c_no, int ser_no, string key, object value)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute($@"UPDATE ABUDF
                                SET [{key}]=@Value
                                WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO",
                                new
                                {
                                    Value = value,
                                    C_NO = c_no,
                                    SER_NO = ser_no
                                }, commandTimeout: 180);

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 取得單筆ABUDF_B
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <returns></returns>
        public ABUDF_B GetABUDF_B(string c_no, int ser_no)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.QueryFirstOrDefault<ABUDF_B>(@"
                    SELECT * FROM ABUDF_B WHERE C_NO=@C_NO AND SER_NO=@SER_NO",
                    new { C_NO = c_no, SER_NO = ser_no });

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 寫入ABUDF_B
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddABUDF_B(FormView form)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                var formB = form.FormB;
                DateTime now = DateTime.Now;
                double applyWorkDays = (formB.E_DATE.ToWestDate() - formB.B_DATE.ToWestDate()).TotalDays + 1;
                double calcWorkDays = (form.FormB.E_DATE.ToWestDate() - form.FormB.B_DATE.ToWestDate()).TotalDays + 1;
                double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);
                // 結算狀態
                string B_STAT;
                if (!string.IsNullOrEmpty(form.AP_DATE1))
                    B_STAT = "Z已申報結算";
                else if (form.P_KIND == "一次全繳")
                    B_STAT = "A一次繳清無結算";
                else
                    B_STAT = "B分期繳交待結算";

                // 施工狀態
                string B_CSTAT;
                if (!string.IsNullOrEmpty(form.AP_DATE1))
                    B_CSTAT = "D結算完工";
                else if (form.AP_DATE.ToWestDate() < form.B_DATE.ToWestDate())
                    B_CSTAT = "A預計工期未施工";
                else
                    B_CSTAT = "B預計工期施工中";

                // 計算各項差異
                double diffDays = (calcWorkDays - downDays) - applyWorkDays; // 工期差
                double diffArea = (form.FormB.AREA ?? 0) - (form.AREA ?? 0);   // 面積差
                double diffMoney = (form.FormB.MONEY ?? 0) - (form.MONEY);     // 經費差
                double diffVOLUMEL = (form.FormB.VOLUMEL ?? 0) - (form.VOLUMEL ?? 0); // 土石外運差

                // 更新異動欄位並收集異動字串
                List<string> changes = new List<string>();

                // 工期異動
                form.FormB.CHG_DATE = diffDays > 0 ? "工期延長" : (diffDays < 0 ? "工期縮短" : null);
                if (!string.IsNullOrEmpty(form.FormB.CHG_DATE)) changes.Add(form.FormB.CHG_DATE);

                // 面積異動
                form.FormB.CHG_AREA = diffArea > 0 ? "工程面積增加" : (diffArea < 0 ? "工程面積減少" : null);
                if (!string.IsNullOrEmpty(form.FormB.CHG_AREA)) changes.Add(form.FormB.CHG_AREA);

                // 經費異動
                form.FormB.CHG_MONEY = diffMoney > 0 ? "合約經費增加" : (diffMoney < 0 ? "合約經費減少" : null);
                if (!string.IsNullOrEmpty(form.FormB.CHG_MONEY)) changes.Add(form.FormB.CHG_MONEY);

                // 土石外運異動
                form.FormB.CHG_VOLUMEL = diffVOLUMEL > 0 ? "土石外運增加" : (diffVOLUMEL < 0 ? "土石外運減少" : null);
                if (!string.IsNullOrEmpty(form.FormB.CHG_VOLUMEL)) changes.Add(form.FormB.CHG_VOLUMEL);

                // 用 "_" 組合所有異動項目存入 CHG_COMM
                form.FormB.CHG_COMM = changes.Any() ? string.Join("_", changes) : null;

                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"DELETE FROM ABUDF_B WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO",
                            new { C_NO = formB.C_NO, SER_NO = formB.SER_NO }, commandTimeout: 180);

                    cn.Execute(@"
                        INSERT INTO ABUDF_B (
                            [C_NO],
                            [SER_NO],
                            [AP_DATE1],
                            [B_STAT],
                            [B_CSTAT],
                            [KIND_NO],
                            [KIND],
                            [YEAR],
                            [A_KIND],
                            [MONEY],
                            [AREA],
                            [VOLUMEL],
                            [B_DAY],
                            [B_DATE],
                            [E_DATE],
                            [B_YEAR],
                            [S_AMT],
                            [CHG_MONEY],
                            [CHG_AREA],
                            [CHG_VOLUMEL],
                            [CHG_DATE],
                            [CHG_COMM],
                            [B_KIND1],
                            [T_DAY],
                            [PRE_C_AMT],
                            [PRE_C_AMT1],
                            [OPINION],
                            [WRONG_AP],
                            [KEYIN],
                            [C_DATE],
                            [M_DATE]
                        )
                        VALUES (
                            @C_NO,
                            @SER_NO,
                            @AP_DATE1,
                            @B_STAT,
                            @B_CSTAT,
                            @KIND_NO,
                            @KIND,
                            @YEAR,
                            @A_KIND,
                            @MONEY,
                            @AREA,
                            @VOLUMEL,
                            @B_DAY,
                            @B_DATE,
                            @E_DATE,
                            @B_YEAR,
                            @S_AMT,
                            @CHG_MONEY,
                            @CHG_AREA,
                            @CHG_VOLUMEL,
                            @CHG_DATE,
                            @CHG_COMM,
                            @B_KIND1,
                            @T_DAY,
                            @PRE_C_AMT,
                            @PRE_C_AMT1,
                            @OPINION,
                            @WRONG_AP,
                            @KEYIN,
                            @C_DATE,
                            @M_DATE
                        )",
                        new
                        {
                            C_NO = formB.C_NO,
                            SER_NO = formB.SER_NO,
                            AP_DATE1 = formB.AP_DATE1,
                            B_STAT = B_STAT,
                            B_CSTAT = B_CSTAT,
                            KIND_NO = formB.KIND_NO,
                            KIND = formB.KIND,
                            YEAR = formB.YEAR,
                            A_KIND = formB.A_KIND,
                            MONEY = (formB.MONEY ?? 0) - (formB.TAX_MONEY ?? 0),
                            AREA = formB.AREA,
                            VOLUMEL = formB.VOLUMEL,
                            B_DAY = downDays,
                            B_DATE = formB.B_DATE,
                            E_DATE = formB.E_DATE,
                            B_YEAR = Math.Round((applyWorkDays - downDays + 1) / 365, 2, MidpointRounding.AwayFromZero),
                            S_AMT = formB.S_AMT,
                            CHG_MONEY = formB.CHG_MONEY,
                            CHG_AREA = formB.CHG_AREA,
                            CHG_VOLUMEL = formB.CHG_VOLUMEL,
                            CHG_DATE = formB.CHG_DATE,
                            CHG_COMM = formB.CHG_COMM,
                            B_KIND1 = formB.B_KIND1,
                            T_DAY = applyWorkDays - downDays + 1,
                            PRE_C_AMT = form.S_AMT > form.S_AMT2 ? form.S_AMT - form.S_AMT2 : 0,
                            PRE_C_AMT1 = form.S_AMT2 > form.S_AMT ? form.S_AMT2 - form.S_AMT : 0,
                            OPINION = formB.OPINION,
                            WRONG_AP = formB.WRONG_AP,
                            KEYIN = "EPB02",
                            C_DATE = now.ToString("yyyy-MM-dd HH:mm:ss"),
                            M_DATE = now.ToString("yyyy-MM-dd HH:mm:ss")
                        },
                        commandTimeout: 180);

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 取得聯單序號
        /// </summary>
        /// <param name="pdate">填單日期</param>
        /// <returns></returns>
        public string GetFLNo(string pdate)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                int no = 1;
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    var result = cn.QueryFirstOrDefault(@"
                        SELECT TOP 1 * FROM ABUDF_1
                        WHERE P_DATE=@P_DATE AND LEFT(FLNO,4)=@BotCode
                        ORDER BY C_DATE DESC, M_DATE DESC",
                        new { P_DATE = pdate, BotCode = base.botCode }, commandTimeout: 180);

                    if (result == null || result.FLNO == "" || !int.TryParse(result.FLNO, out int flno))
                    {
                        no = 1;
                    }
                    else
                    {
                        no = int.Parse(result.FLNO.Substring(result.FLNO.Length - 3, 3));
                    }

                    return pdate.Substring(2, 3) + no.ToString().PadLeft(3, '0');
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 取得單筆ABUDF_I
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <returns></returns>
        public List<ABUDF_I> GetABUDF_I(string c_no, int ser_no)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.Query<ABUDF_I>(@"
                    SELECT * FROM ABUDF_I WHERE C_NO=@C_NO AND SER_NO=@SER_NO",
                    new { C_NO = c_no, SER_NO = ser_no }).ToList();

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 查詢 ABUDF_1
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public ABUDF_1 GetABUDF_1(Form form)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    var result = cn.QueryFirstOrDefault<ABUDF_1>(@"
                        SELECT TOP 1 * FROM ABUDF_1
                        WHERE C_NO=@C_NO AND SER_NO=@SER_NO AND P_TIME=@P_TIME",
                        new
                        {
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO,
                            P_TIME = string.IsNullOrEmpty(form.AP_DATE1) ? "01" : "02"
                        }, commandTimeout: 180);

                    return result;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 取得單筆ABUDF_1
        /// </summary>
        /// <param name="c_no"></param>
        /// <param name="ser_no"></param>
        /// <returns></returns>
        public List<ABUDF_1> GetABUDF_1(string c_no, int ser_no)
        {
#if !DEBUG
            using (var impersonation = new ImpersonationContext(domain, userName, password))
            {
#endif
            using (var cn = new OleDbConnection(accessConnStr))
            {
                var result = cn.Query<ABUDF_1>(@"
                    SELECT * FROM ABUDF_1 WHERE C_NO=@C_NO AND SER_NO=@SER_NO",
                    new { C_NO = c_no, SER_NO = ser_no }).ToList();

                return result;
            }
#if !DEBUG
            }
#endif
        }

        /// <summary>
        /// 寫入ABUDF_1
        /// </summary>
        /// <param name="abudf_1"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddABUDF_1(ABUDF_1 abudf_1)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    // ABUDF_1關聯到ABUDF_I，所以要先刪除ABUDF_I
                    cn.Execute(@"
                        DELETE FROM ABUDF_I WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO AND [P_TIME]=@P_TIME",
                        new
                        {
                            C_NO = abudf_1.C_NO,
                            SER_NO = abudf_1.SER_NO,
                            P_TIME = abudf_1.P_TIME
                        }, commandTimeout: 180);

                    cn.Execute(@"DELETE FROM ABUDF_1 WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO AND [P_TIME]=@P_TIME",
                        new
                        {
                            C_NO = abudf_1.C_NO,
                            SER_NO = abudf_1.SER_NO,
                            P_TIME = abudf_1.P_TIME
                        }, commandTimeout: 180);

                    cn.Execute(@"
                        INSERT INTO ABUDF_1 (
                            [C_NO],
                            [SER_NO],
                            [P_TIME],
                            [P_DATE],
                            [E_DATE],
                            [FLNO],
                            [F_AMT],
                            [B_AMT],
                            [KEYIN],
                            [C_DATE],
                            [M_DATE]
                        )
                        VALUES (
                            @C_NO,
                            @SER_NO,
                            @P_TIME,
                            @P_DATE,
                            @E_DATE,
                            @FLNO,
                            @F_AMT,
                            @B_AMT,
                            @KEYIN,
                            @C_DATE,
                            @M_DATE
                        )",
                        new
                        {
                            C_NO = abudf_1.C_NO,
                            SER_NO = abudf_1.SER_NO,
                            P_TIME = abudf_1.P_TIME,
                            P_DATE = abudf_1.P_DATE,
                            E_DATE = abudf_1.E_DATE,
                            FLNO = abudf_1.FLNO,
                            F_AMT = abudf_1.F_AMT,
                            B_AMT = abudf_1.B_AMT,
                            KEYIN = "EPB02",
                            C_DATE = abudf_1.C_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            M_DATE = abudf_1.M_DATE.ToString("yyyy-MM-dd HH:mm:ss")
                        },
                        commandTimeout: 180);

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 更新ABUDF_1
        /// </summary>
        /// <param name="abudf_1"></param>
        /// <param name="lastFLNO">最新的銷帳編號</param>
        /// <returns></returns>
        public bool UpdateABUDF_1(ABUDF_1 abudf_1, string lastFLNO)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"
                        UPDATE ABUDF_1
                            SET [FLNO]=@FLNO,
                                [F_DATE]=@F_DATE,
                                [F_AMT]=@F_AMT,
                                [PM_DATE]=@PM_DATE,
                                [A_DATE]=@A_DATE,
                                [M_DATE]=@M_DATE
                        WHERE [FLNO]=@lastFLNO",
                        new
                        {
                            FLNO = abudf_1.FLNO,
                            F_DATE = abudf_1.F_DATE,
                            F_AMT = abudf_1.F_AMT,
                            PM_DATE = abudf_1.PM_DATE,
                            A_DATE = abudf_1.A_DATE,
                            M_DATE = abudf_1.M_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            lastFLNO = lastFLNO
                        }, commandTimeout: 180);

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 寫入ABUDF_I
        /// </summary>
        /// <param name="abudf_I"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public bool AddABUDF_I(ABUDF_I abudf_I)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"
                        DELETE FROM ABUDF_I 
                        WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO AND [P_TIME]=@P_TIME",
                        new
                        {
                            C_NO = abudf_I.C_NO,
                            SER_NO = abudf_I.SER_NO,
                            P_TIME = abudf_I.P_TIME
                        }, commandTimeout: 180);

                    cn.Execute(@"
                        INSERT INTO ABUDF_I (
                            [C_NO],
                            [SER_NO],
                            [P_TIME],
                            [S_DATE],
                            [E_DATE],
                            [PERCENT],
                            [F_AMT],
                            [I_AMT],
                            [PEN_AMT],
                            [PEN_RATE],
                            [KEYIN],
                            [C_DATE],
                            [M_DATE]
                        )
                        VALUES (
                            @C_NO,
                            @SER_NO,
                            @P_TIME,
                            @S_DATE,
                            @E_DATE,
                            @PERCENT,
                            @F_AMT,
                            @I_AMT,
                            @PEN_AMT,
                            @PEN_RATE,
                            @KEYIN,
                            @C_DATE,
                            @M_DATE
                        )",
                        new
                        {
                            C_NO = abudf_I.C_NO,
                            SER_NO = abudf_I.SER_NO,
                            P_TIME = abudf_I.P_TIME,
                            S_DATE = abudf_I.S_DATE,
                            E_DATE = abudf_I.E_DATE,
                            PERCENT = abudf_I.PERCENT,
                            F_AMT = abudf_I.F_AMT,
                            I_AMT = abudf_I.I_AMT,
                            PEN_AMT = abudf_I.PEN_AMT,
                            PEN_RATE = abudf_I.PEN_RATE,
                            KEYIN = "EPB02",
                            C_DATE = abudf_I.C_DATE.ToString("yyyy-MM-dd HH:mm:ss"),
                            M_DATE = abudf_I.M_DATE.ToString("yyyy-MM-dd HH:mm:ss")
                        }, commandTimeout: 180);

                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }

        /// <summary>
        /// 寫入ABUDFDay
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool AddABUDFDay(FormView form)
        {
            try
            {
#if !DEBUG
                using (var impersonation = new ImpersonationContext(domain, userName, password))
                {
#endif
                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"
                        DELETE FROM ABUDF_DAY WHERE [C_NO]=@C_NO AND [SER_NO]=@SER_NO",
                        new
                        {
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO
                        }, commandTimeout: 180);

                    foreach (var item in form.StopWorks)
                    {
                        cn.Execute(@"
                            INSERT INTO ABUDF_DAY ([C_NO],[SER_NO],[DOWN_DATE],[UP_DATE],[DOWN_DAY],[KEYIN],[C_DATE],[M_DATE])
                            VALUES (@C_NO, @SER_NO, @DOWN_DATE, @UP_DATE, @DOWN_DAY, @KEYIN, @C_DATE, @M_DATE)",
                            new
                            {
                                C_NO = form.C_NO,
                                SER_NO = form.SER_NO,
                                DOWN_DATE = item.DOWN_DATE,
                                UP_DATE = item.UP_DATE,
                                DOWN_DAY = item.DOWN_DAY,
                                KEYIN = "EPB02",
                                C_DATE = item.C_DATE?.ToString("yyyy-MM-dd HH:mm:ss"),
                                M_DATE = item.M_DATE?.ToString("yyyy-MM-dd HH:mm:ss")
                            }, commandTimeout: 180);
                    }
                    return true;
                }
#if !DEBUG
                }
#endif
            }
            catch (Exception ex)
            {
                Logger.Error(ex.StackTrace);
                throw ex;
            }
        }
    }
}