using NT_AirPollution.Model.Enum;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessWorker
{
    internal class Program
    {
        static FormService _formService = new FormService();
        static AccessService _accessService = new AccessService();
        static void Main(string[] args)
        {
            try
            {
                var forms = _formService.GetFormByAccessWorker();
                foreach (var form in forms)
                {
                    string fileName = "";
                    if(form.VerifyStage1 == VerifyStage.複審通過 && form.FormStatus == FormStatus.通過待繳費)
                    {
                        fileName = $"繳款單{form.C_NO}-{form.SER_NO}({(form.P_KIND == "一次繳清" ? "一次繳清" : "第一期")})";
                    }
                    if (form.VerifyStage2 == VerifyStage.複審通過 && form.CalcStatus == CalcStatus.通過待繳費)
                    {
                        fileName = $"繳款單{form.C_NO}-{form.SER_NO}(結算補繳)";
                    }

                    string pdfPath = _formService.CreatePaymentPDF(fileName, form);
                    Console.WriteLine($@"產生繳款單: {pdfPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
