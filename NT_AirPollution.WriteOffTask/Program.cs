using NT_AirPollution.Model.Access;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.WriteOffTask
{
    internal class Program
    {
        private static string _bankFile = ConfigurationManager.AppSettings["BankFile"].ToString();
        private static string _bankFileHistory = ConfigurationManager.AppSettings["BankFileHistory"].ToString();
        private static FormService _formService = new FormService();
        private static AccessService _accessService = new AccessService();
        static void Main(string[] args)
        {
            LoadBankFile();
        }

        /// <summary>
        /// 銷帳
        /// </summary>
        private static void LoadBankFile()
        {
            string[] fileEntries = Directory.GetFiles(_bankFile);
            foreach (string file in fileEntries)
            {
                string[] line = File.ReadAllLines(file);
                for (int i = 0; i < line.Length - 1; i++)
                {
                    string account = line[i].Substring(8, 16);
                    int payAmount = Convert.ToInt32(line[i].Substring(100, 10));
                    DateTime payDate = Convert.ToDateTime($"{2011 + Convert.ToInt32(line[i].Substring(93, 2))}-{line[i].Substring(95, 2)}-{line[i].Substring(97, 2)}");

                    var payment = new Payment
                    {
                        PaymentID = account,
                        PayAmount = payAmount,
                        PayDate = payDate,
                        BankLog = line[i]
                    };

                    var abudf_1 = new ABUDF_1
                    {
                        F_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd"),
                        F_AMT = payAmount,
                        PM_DATE = payDate.AddYears(-1911).ToString("yyyMMdd"),
                        A_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd"),
                        M_DATE = DateTime.Now,
                        FLNO = account
                    };

                    var isAccessOK = _accessService.UpdateABUDF_1(abudf_1);
                    if (!isAccessOK)
                        return;

                    _formService.UpdatePayment(payment);
                }

                string fileName = Path.GetFileName(file);
                if (File.Exists($@"{_bankFileHistory}\{fileName}"))
                {
                    File.Delete($@"{_bankFileHistory}\{fileName}");
                }
                File.Move(file, $@"{_bankFileHistory}\{fileName}");
            }
        }
    }
}
