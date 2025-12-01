using AutoMapper;
using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessToSQLServer
{
    internal class Program
    {
        static readonly AccessService _accessService = new AccessService();
        static readonly SQLService _sqlService = new SQLService();

        static void Main(string[] args)
        {
            //OneToOne();
            ManyToMany();
        }

        private static void OneToOne()
        {
            string c_no = "M114MDZ109";
            int ser_no = 1;

            ABUDF abudf = _accessService.GetABUDF(c_no, ser_no);


            // 1. 設定 AutoMapper 配置
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ABUDF, Form>());
            var mapper = config.CreateMapper();
            var form = mapper.Map<Form>(abudf);

            form.ClientUserID = 5;
            form.CreateUserEmail = "han109ww@gmail.com";
            form.CreateUserName = "陳秋涵";
            form.FormStatus = FormStatus.已繳費完成;
            form.VerifyDate1 = DateTime.Now;
            form.VerifyStage1 = VerifyStage.複審通過;
            form.PayEndDate1 = null;
            form.CalcStatus = CalcStatus.未申請;
            form.VerifyDate2 = null;
            form.VerifyStage2 = VerifyStage.未申請;
            long id = _sqlService.AddForm(form);

            for (int i = 1; i <= 2; i++)
            {
                var abudf_1 = _accessService.GetABUDF_1(c_no, ser_no, $"0{i}");
                var abudf_i = _accessService.GetABUDF_I(c_no, ser_no, $"0{i}");

                if (abudf_1 == null) continue;

                Payment payment = new Payment
                {
                    FormID = id,
                    Term = $"{i}",
                    PayEndDate = ChineseDateToWestDate(abudf_1.E_DATE).Value,
                    PaymentID = abudf_1?.FLNO,
                    PayableAmount = abudf.P_AMT,
                    Penalty = abudf_i?.PEN_AMT,
                    Interest = abudf_i?.I_AMT,
                    Percent = abudf_i?.PERCENT ?? 1.725,
                    PayAmount = abudf_1.F_AMT,
                    PayDate = ChineseDateToWestDate(abudf_1.PM_DATE),
                    CreateDate = abudf_1.C_DATE,
                    ModifyDate = abudf_1.M_DATE
                };

                _sqlService.AddPayment(payment);
            }
        }

        private static void ManyToMany()
        {
            DateTime now = DateTime.Now;
            var allUser = _sqlService.GetClientUser().ToList();
            var allABUDF = _accessService.GetABUDF().ToList();
            var allABUDF_1 = _accessService.GetABUDF_1().ToList();
            var allABUDF_I = _accessService.GetABUDF_I().ToList();
            var allOldSQL = _sqlService.GetTDMFORMA().ToList();
            var allABUDF_B = _accessService.GetABUDF_B().ToList();

            foreach (var abudf in allABUDF)
            {
                // 1. 設定 AutoMapper 配置
                var config = new MapperConfiguration(cfg => cfg.CreateMap<ABUDF, Form>());
                var mapper = config.CreateMapper();
                var form = mapper.Map<Form>(abudf);

                var oldSQL = allOldSQL.FirstOrDefault(o => o.C_NO == abudf.C_NO && o.SER_NO == $"{abudf.SER_NO}");
                var user = allUser.FirstOrDefault(u => u.Email == oldData.DSG_EUSR_NAME);
                var abudf1 = allABUDF_1.FirstOrDefault(o => o.C_NO == abudf.C_NO && o.SER_NO == abudf.SER_NO && o.P_TIME == "01");
                var abudf_b = allABUDF_B.FirstOrDefault(o => o.C_NO == abudf.C_NO && o.SER_NO == abudf.SER_NO);

                form.ClientUserID = user.ID;
                form.CreateUserEmail = oldSQL.DSG_EUSR_NAME;
                form.CreateUserName = oldSQL.DSG_EUSR_NAME;

                /* 申請狀態 */
                form.VerifyDate1 = DateTime.Now;
                form.VerifyStage1 = VerifyStage.複審通過;
                form.PayEndDate1 = null;

                if (abudf.S_AMT > 0 && string.IsNullOrEmpty(abudf.FIN_DATE))
                    form.FormStatus = FormStatus.通過待繳費;
                else if (abudf.S_AMT == 0 && string.IsNullOrEmpty(abudf.FIN_DATE))
                    form.FormStatus = FormStatus.免繳費;
                else if (abudf1 != null && !string.IsNullOrEmpty(abudf1.F_DATE))
                    form.FormStatus = FormStatus.已繳費完成;


                /* 結算狀態 */
                form.PayEndDate2 = null;
                form.CalcStatus = CalcStatus.未申請;

                if (!string.IsNullOrEmpty(abudf_b.AP_DATE1))
                {
                    form.CalcStatus = CalcStatus.通過待繳費;
                    form.VerifyDate2 = ChineseDateToWestDate(abudf_b.AP_DATE1);
                    form.VerifyStage2 = VerifyStage.複審通過;
                }
                
                if (!string.IsNullOrEmpty(abudf_b.AP_DATE1) && abudf_b.PRE_C_AMT < 4000)
                    form.CalcStatus = CalcStatus.通過待退費小於4000;
                else if (!string.IsNullOrEmpty(abudf_b.AP_DATE1) && abudf_b.PRE_C_AMT >= 4000)
                    form.CalcStatus = CalcStatus.通過待退費大於4000;
                else if (!string.IsNullOrEmpty(abudf.FIN_DATE))
                    form.CalcStatus = CalcStatus.繳退費完成;


                long id = _sqlService.AddForm(form);

                for (int i = 1; i <= 2; i++)
                {
                    var abudf_1 = allABUDF_1.FirstOrDefault(o => o.C_NO == abudf.C_NO && o.SER_NO == abudf.SER_NO && o.P_TIME == $"0{i}");
                    var abudf_i = allABUDF_I.FirstOrDefault(o => o.C_NO == abudf.C_NO && o.SER_NO == abudf.SER_NO && o.P_TIME == $"0{i}");

                    if (abudf_1 == null) continue;

                    Payment payment = new Payment
                    {
                        FormID = id,
                        Term = $"{i}",
                        PayEndDate = ChineseDateToWestDate(abudf_1.E_DATE).Value,
                        PaymentID = abudf_1?.FLNO,
                        PayableAmount = abudf.P_AMT,
                        Penalty = abudf_i?.PEN_AMT,
                        Interest = abudf_i?.I_AMT,
                        Percent = abudf_i?.PERCENT ?? 1.725,
                        PayAmount = abudf_1.F_AMT,
                        PayDate = ChineseDateToWestDate(abudf_1.PM_DATE),
                        CreateDate = abudf_1.C_DATE,
                        ModifyDate = abudf_1.M_DATE
                    };

                    _sqlService.AddPayment(payment);
                }
            }
        }

        static DateTime? ChineseDateToWestDate(string dt)
        {
            if (string.IsNullOrEmpty(dt)) return null;
            return Convert.ToDateTime($"{Convert.ToInt32(dt.Substring(0, 3)) + 1911}-{dt.Substring(3, 2)}-{dt.Substring(5, 2)}");
        }
    }
}
