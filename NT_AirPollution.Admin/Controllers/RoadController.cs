using ClosedXML.Excel;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Hosting;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    public class RoadController : ApiController
    {
        private readonly RoadService _roadService = new RoadService();

        [HttpPost]
        public List<RoadExcel> GetRoadReport(RoadFilter filter)
        {
            return _roadService.GetRoadReport(filter);
        }

        [HttpPost]
        public string ExportRoadReport(RoadFilter filter)
        {
            var result = _roadService.GetRoadReport(filter);
            var wb = new XLWorkbook(HostingEnvironment.MapPath(@"~/App_Data/Template/RoadReportTemplate.xlsx"));
            var ws = wb.Worksheet(1);
            int rowIdx = 2;
            foreach (var item in result)
            {
                ws.Cell(rowIdx, 1).SetValue(item.C_NO);
                ws.Cell(rowIdx, 2).SetValue(item.SER_NO);
                ws.Cell(rowIdx, 3).SetValue(item.COMP_NAM);
                ws.Cell(rowIdx, 4).SetValue(item.TOWN_NA);
                ws.Cell(rowIdx, 5).SetValue(item.KIND);
                ws.Cell(rowIdx, 6).SetValue(item.C_DATE.ToString("yyyy-MM-dd"));
                ws.Cell(rowIdx, 7).SetValue(item.S_NAME);
                ws.Cell(rowIdx, 8).SetValue(item.PromiseCreateDate.ToString("yyyy-MM-dd"));
                ws.Cell(rowIdx, 9).SetValue(item.RoadLength);
                ws.Cell(rowIdx, 10).SetValue(item.RoadName);
                ws.Cell(rowIdx, 11).SetValue(item.StartDate.ToString("yyyy-MM-dd"));
                ws.Cell(rowIdx, 12).SetValue(item.EndDate.ToString("yyyy-MM-dd"));
                int colIdx = 12;
                for (int i = 1; i <= 12; i++)
                {
                    var month = item.RoadExcelMonth.FirstOrDefault(o => o.Month == i);
                    if (month == null)
                    {
                        ws.Cell(rowIdx, ++colIdx).SetValue(0);
                        ws.Cell(rowIdx, ++colIdx).SetValue(0);
                    }
                    else
                    {
                        ws.Cell(rowIdx, ++colIdx).SetValue(month.CleanLength1);
                        ws.Cell(rowIdx, ++colIdx).SetValue(month.CleanLength2);
                    }
                }
                ws.Cell(rowIdx, ++colIdx).SetValue(item.CleanWay1);
                ws.Cell(rowIdx, ++colIdx).SetValue(item.CleanWay2);
                rowIdx++;
            }

            ws.Columns().AdjustToContents(); ;
            string fileName = $"{Guid.NewGuid().ToString()}.xlsx";
            wb.SaveAs(HostingEnvironment.MapPath($@"~/App_Data/Download/{fileName}"));
            return fileName;
        }
    }
}
