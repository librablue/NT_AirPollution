using Dapper;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
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
                        INSERT INTO ABUDF ([C_NO],[SER_NO],[COUNTRY],[TOWN_NO],[TOWN_NA],[KIND_NO],[KIND],[AP_DATE],[B_SERNO],[PUB_COMP],[S_NAME],[S_G_NO],[S_ADDR1],[S_ADDR2],[S_TEL],[S_B_NAM],[S_B_TIT],[S_B_ID],[S_B_BDATE],[S_C_NAM],[S_C_TIT],[S_C_ID],[S_C_ADDR],[S_C_TEL],[R_NAME],[R_G_NO],[R_ADDR1],[R_ADDR2],[R_TEL],[R_B_NAM],[R_B_TIT],[R_B_ID],[R_B_BDATE],[R_ADDR3],[R_TEL1],[R_M_NAM],[R_C_NAM],[A_KIND],[AREA],[VOLUMEL],[RATIOLB],[DENSITYL],[MONEY],[C_MONEY],[PERCENT],[YEAR],[B_DATE],[E_DATE],[S_AMT],[P_KIND],[P_NUM],[P_AMT],[FIN_DATE],[FIN_COM],[STATE],[ID_DOC1],[ID_DOC2],[ID_DOC3],[COMP_DOC1],[COMP_DOC2],[COMP_DOC3],[COMP_OTH],[BUD_DOC1],[BUD_DOC2],[BUD_DOC3],[BUD_OTH],[REC_YN],[AREA_B],[AREA_F],[PERC_B],[COMP_NAM],[ADDR],[AP_TYPE],[UTME],[UTMN],[LATLNG],[AREA3],[AREA2],[O_C_Q],[G_NAME],[ADDR1],[COMP_NAM1],[ENG_STONE],[ENG_WRAP],[ENG_DES],[ENG_DES_TEL],[ENG_DES_ADDR],[NOLEVY],[COMP_L],[RCAP_DATE],[RC_DATE],[RC_SERNO],[RCE],[COMMENT],[EIACOMMENTS],[C_DATE],[M_DATE],[KEYIN],[RECCOMMENTS])
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
                                C_DATE = form.C_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                M_DATE = form.M_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                KEYIN = "EPB02",
                                RECCOMMENTS = form.RECCOMMENTS
                            }, commandTimeout: 180);

                    // 20240115說前台不用停復工
                    //foreach (var item in form.StopWorks)
                    //{
                    //    cn.Execute(@"
                    //        INSERT INTO ABUDF_DAY ([C_NO],[SER_NO],[DOWN_DATE],[UP_DATE],[DOWN_DAY],[KEYIN],[C_DATE],[M_DATE])
                    //        VALUES (?,?,?,?,?,?,?,?)",
                    //        new
                    //        {
                    //            C_NO = form.C_NO,
                    //            SER_NO = form.SER_NO,
                    //            DOWN_DATE = item.DOWN_DATE,
                    //            UP_DATE = item.UP_DATE,
                    //            DOWN_DAY = item.DOWN_DAY,
                    //            KEYIN = "EPB02",
                    //            C_DATE = item.C_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    //            M_DATE = item.M_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    //        });
                    //}

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
                                    VOLUMEL = form.VOLUMEL ?? 0,
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
                                    M_DATE = form.M_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                    C_NO = form.C_NO,
                                    SER_NO = form.SER_NO
                                }, commandTimeout: 180);


                    // 20240115說前台不用停復工
                    //cn.Execute(@"DELETE FROM ABUDF_DAY WHERE [C_NO]=? AND [SER_NO]=?",
                    //    new { C_NO = form.C_NO, SER_NO = form.SER_NO });

                    //foreach (var item in form.StopWorks)
                    //{
                    //    cn.Execute(@"
                    //        INSERT INTO ABUDF_DAY ([C_NO],[SER_NO],[DOWN_DATE],[UP_DATE],[DOWN_DAY],[KEYIN],[C_DATE],[M_DATE])
                    //        VALUES (?,?,?,?,?,?,?,?)",
                    //        new
                    //        {
                    //            C_NO = form.C_NO,
                    //            SER_NO = form.SER_NO,
                    //            DOWN_DATE = item.DOWN_DATE,
                    //            UP_DATE = item.UP_DATE,
                    //            DOWN_DAY = item.DOWN_DAY,
                    //            KEYIN = "EPB02",
                    //            C_DATE = item.C_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    //            M_DATE = item.M_DATE.Value.ToString("yyyy-MM-dd HH:mm:ss")
                    //        });
                    //}

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
                double workDays = (base.ChineseDateToWestDate(form.E_DATE) - base.ChineseDateToWestDate(form.B_DATE)).TotalDays + 1;
                double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);
                string B_STAT;
                if (form.P_KIND == "一次全繳")
                    B_STAT = "A一次繳清無結算";
                else
                    B_STAT = "B分期繳交待結算";

                if (form.S_AMT <= 100)
                    B_STAT = "Z已申報結算";

                using (var cn = new OleDbConnection(accessConnStr))
                {
                    cn.Execute(@"DELETE FROM ABUDF_B WHERE [C_NO]=? AND [SER_NO]=?",
                            new { C_NO = form.C_NO, SER_NO = form.SER_NO }, commandTimeout: 180);

                    cn.Execute(@"
                        INSERT INTO ABUDF_B ([C_NO],[SER_NO],[AP_DATE1],[B_STAT],[KIND_NO],[KIND],[YEAR],[A_KIND],[MONEY],[AREA],[VOLUMEL],[B_DAY],[B_DATE],[E_DATE],[S_AMT],[T_DAY],[PRE_C_AMT],[PRE_C_AMT1],[KEYIN],[C_DATE],[M_DATE])
                        VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)",
                        new
                        {
                            C_NO = form.C_NO,
                            SER_NO = form.SER_NO,
                            AP_DATE1 = form.AP_DATE1,
                            B_STAT = B_STAT,
                            KIND_NO = form.KIND_NO,
                            KIND = form.KIND,
                            YEAR = form.YEAR,
                            A_KIND = form.A_KIND,
                            MONEY = form.MONEY,
                            AREA = form.AREA,
                            VOLUMEL = form.VOLUMEL,
                            B_DAY = downDays,
                            B_DATE = form.B_DATE,
                            E_DATE = form.E_DATE,
                            S_AMT = form.S_AMT,
                            T_DAY = workDays - downDays + 1,
                            PRE_C_AMT = form.S_AMT > form.S_AMT2 ? form.S_AMT - form.S_AMT2 : 0,
                            PRE_C_AMT1 = form.S_AMT2 > form.S_AMT ? form.S_AMT2 - form.S_AMT : 0,
                            KEYIN = "EPB02",
                            C_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            M_DATE = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
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
                        DELETE FROM ABUDF_I WHERE [C_NO]=? AND [SER_NO]=? AND [P_TIME]=?",
                        new
                        {
                            C_NO = abudf_1.C_NO,
                            SER_NO = abudf_1.SER_NO,
                            P_TIME = abudf_1.P_TIME
                        }, commandTimeout: 180);

                    cn.Execute(@"DELETE FROM ABUDF_1 WHERE [C_NO]=? AND [SER_NO]=? AND [P_TIME]=?",
                        new
                        {
                            C_NO = abudf_1.C_NO,
                            SER_NO = abudf_1.SER_NO,
                            P_TIME = abudf_1.P_TIME
                        }, commandTimeout: 180);

                    cn.Execute(@"
                        INSERT INTO ABUDF_1 ([C_NO],[SER_NO],[P_TIME],[P_DATE],[E_DATE],[FLNO],[F_AMT],[B_AMT],[KEYIN],[C_DATE],[M_DATE])
                        VALUES (?,?,?,?,?,?,?,?,?,?,?)",
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
                            abudf_1.FLNO,
                            abudf_1.F_DATE,
                            abudf_1.F_AMT,
                            abudf_1.PM_DATE,
                            abudf_1.A_DATE,
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
                        DELETE FROM ABUDF_I WHERE [C_NO]=? AND [SER_NO]=? AND [P_TIME]=?",
                        new
                        {
                            C_NO = abudf_I.C_NO,
                            SER_NO = abudf_I.SER_NO,
                            P_TIME = abudf_I.P_TIME
                        }, commandTimeout: 180);

                    cn.Execute(@"
                        INSERT INTO ABUDF_I ([C_NO],[SER_NO],[P_TIME],[S_DATE],[E_DATE],[PERCENT],[F_AMT],[I_AMT],[PEN_AMT],[PEN_RATE],[KEYIN],[C_DATE],[M_DATE])
                        VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?)",
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
                            PEN_RATE = abudf_I.F_AMT,
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
    }
}