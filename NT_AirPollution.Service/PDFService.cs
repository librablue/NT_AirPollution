using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TuesPechkin;

namespace NT_AirPollution.Service
{
    public class PDFService
    {
        private static IConverter _Converter = new ThreadSafeConverter(
            new RemotingToolset<PdfToolset>(
                new WinAnyCPUEmbeddedDeployment(
                    new TempFolderDeployment())));

        public static IConverter Converter
        {
            get
            {
                return _Converter;
            }
        }
    }
}
