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
                    PayableAmount = abudf.P_AMT.Value,
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

        static DateTime? ChineseDateToWestDate(string dt)
        {
            if (string.IsNullOrEmpty(dt)) return null;
            return Convert.ToDateTime($"{Convert.ToInt32(dt.Substring(0, 3)) + 1911}-{dt.Substring(3, 2)}-{dt.Substring(5, 2)}");
        }
    }
}
