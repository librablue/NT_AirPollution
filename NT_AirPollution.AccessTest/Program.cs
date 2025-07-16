using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.AccessTest
{
    internal class Program
    {
        static AccessService _accessService = new AccessService();
        static void Main(string[] args)
        {
            try
            {
                var result = _accessService.GetC_NO();
                foreach (var item in result)
                {
                    Console.WriteLine(item.C_NO);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            Console.ReadLine();
        }
    }
}
