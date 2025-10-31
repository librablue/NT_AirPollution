using ClosedXML.Excel;
using NT_AirPollution.Admin.ActionFilter;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web;
using System.Web.Http;

namespace NT_AirPollution.Admin.Controllers
{
    [CustomAuthorize]
    public class FormController : ApiController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly FormService _formService = new FormService();
        private readonly AccessService _accessService = new AccessService();
        private readonly OptionService _optionService = new OptionService();

        [HttpPost]
        public List<FormView> GetForms(FormFilter filter)
        {
            var forms = _formService.GetForms(filter);
            return forms;
        }

        /// <summary>
        /// 複製表單追加序號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public bool CopyForm(FormView form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                if (_formService.ChineseDateToWestDate(form.B_DATE) > _formService.ChineseDateToWestDate(form.E_DATE))
                    throw new Exception("施工期程起始日期不能大於結束日期");

                form.C_DATE = DateTime.Now;
                form.M_DATE = DateTime.Now;
                form.FormStatus = FormStatus.審理中;
                form.CalcStatus = CalcStatus.未申請;

                int currentSER_NO = _accessService.GetMaxSER_NOByC_NO(form);
                form.SER_NO = currentSER_NO + 1;
                // 寫入 Access
                _accessService.AddABUDF(form);

                _formService.AddForm(form);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool UpdateForm(FormView form)
        {
            try
            {
                if (form.KIND_NO == "1" || form.KIND_NO == "2")
                {
                    // 1、2類工程面積=建築面積
                    form.AREA = form.AREA_B;
                    // 建蔽率=(建築面積AREA_B)/(基地面積AREA_F)*100%
                    form.PERC_B = Math.Round((double)(form.AREA_B / form.AREA_F * 100), 2, MidpointRounding.AwayFromZero);
                }
                else if (form.KIND_NO == "3")
                {
                    // 3類工程面積=總樓地板面積
                    form.AREA = form.AREA2;
                }
                else
                {
                    form.AREA_F = null;
                    form.AREA_B = null;
                    form.PERC_B = null;
                }

                // 停工天數
                double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays + 1);
                var result = _formService.CalcTotalMoney(form, downDays);

                // 申報
                if (string.IsNullOrEmpty(form.AP_DATE1))
                {
                    form.S_AMT = result.TotalMoney;
                    // 10000以上才能分期
                    if (form.S_AMT < 10000)
                        form.P_KIND = "一次全繳";

                    form.P_NUM = form.P_KIND == "一次全繳" ? 1 : 2;
                    form.P_AMT = form.S_AMT;
                    if (form.P_KIND == "分兩次繳清")
                        form.P_AMT = Math.Round((form.S_AMT.GetValueOrDefault()) / 2, 0, MidpointRounding.AwayFromZero);

                    // 100元以下免繳
                    if (form.S_AMT <= 100)
                    {
                        form.P_KIND = "一次全繳";
                        form.P_NUM = 1;
                        form.P_AMT = form.S_AMT;
                    }
                }
                // 結算
                else
                {
                    form.S_AMT2 = result.TotalMoney;
                }

                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();

                form.COMP_L = result.Level;
                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;

                // 有管制編號才更新Access
                if (!string.IsNullOrEmpty(form.C_NO))
                {
                    _accessService.UpdateABUDF(form);
                    // 更新ABUDF_B
                    _accessService.AddABUDF_B(form);
                }


                _formService.UpdateForm(form);
                // 更新FormB
                _formService.AddFormB(form);
                _formService.UpdateStopWork(form);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 產生管制編號
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public string CreateC_NO(FormView form)
        {
            try
            {
                string c_no = _accessService.GetC_NO(form);
                form.C_NO = c_no;

                // 寫入 Access
                _accessService.AddABUDF(form);

                _formService.UpdateForm(form);
                _formService.AddFormB(form);
                return form.C_NO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新申請進度
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public bool UpdateStatus(FormView form)
        {
            try
            {
                // 初審
                if (BaseService.CurrentAdmin.RoleID == 1)
                {
                    switch (form.FormStatus)
                    {
                        case FormStatus.待補件:
                            form.VerifyStage1 = VerifyStage.未申請;
                            break;
                        case FormStatus.通過待繳費:
                            // 審核日期
                            form.VerifyDate1 = DateTime.Now;
                            form.VerifyStage1 = VerifyStage.初審通過;
                            break;
                    }

                    switch (form.CalcStatus)
                    {
                        case CalcStatus.待補件:
                            form.VerifyStage2 = VerifyStage.未申請;
                            break;
                        case CalcStatus.通過待繳費:
                            form.VerifyDate2 = DateTime.Now;
                            form.VerifyStage2 = VerifyStage.初審通過;
                            break;
                    }
                }

                // 複審
                if (BaseService.CurrentAdmin.RoleID == 2)
                {
                    switch (form.FormStatus)
                    {
                        case FormStatus.待補件:
                            form.VerifyStage1 = VerifyStage.未申請;
                            break;
                        case FormStatus.通過待繳費:
                            // 審核日期
                            form.VerifyDate1 = DateTime.Now;
                            form.VerifyStage1 = VerifyStage.複審通過;

                            // 100元以下免繳
                            if (form.S_AMT <= 100)
                                form.FormStatus = FormStatus.免繳費;
                            break;
                    }

                    // 3.4.5指令共用，用退費金額<4000判斷4，>=4000判斷5
                    if (form.CalcStatus == CalcStatus.通過待繳費)
                    {
                        // 停工天數
                        double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays + 1);
                        var result = _formService.CalcTotalMoney(form, downDays);
                        form.S_AMT2 = result.TotalMoney;
                        form.COMP_L = result.Level;

                        if (form.S_AMT2 > form.P_AMT)
                            form.CalcStatus = CalcStatus.通過待繳費;
                        else if (form.S_AMT2 == form.P_AMT)
                            form.CalcStatus = CalcStatus.繳退費完成;
                        else if (form.P_AMT - form.S_AMT2 < 4000)
                            form.CalcStatus = CalcStatus.通過待退費小於4000;
                        else if (form.P_AMT - form.S_AMT2 >= 4000)
                            form.CalcStatus = CalcStatus.通過待退費大於4000;

                        // 更新ABUDF FIN_COM(已完成完工查核日期)
                        _accessService.UpdateABUDFByColumn(form.C_NO, form.SER_NO.Value, "FIN_COM", DateTime.Now.AddYears(-1911).ToString("yyyMMdd"));
                    }

                    switch (form.CalcStatus)
                    {
                        case CalcStatus.待補件:
                            form.VerifyStage2 = VerifyStage.未申請;
                            break;
                        case CalcStatus.通過待繳費:
                        case CalcStatus.通過待退費小於4000:
                        case CalcStatus.通過待退費大於4000:
                        case CalcStatus.繳退費完成:
                            form.VerifyDate2 = DateTime.Now;
                            form.VerifyStage2 = VerifyStage.複審通過;

                            if (form.CalcStatus == CalcStatus.通過待退費小於4000 ||
                                form.CalcStatus == CalcStatus.通過待退費大於4000 ||
                                form.CalcStatus == CalcStatus.繳退費完成)
                                form.FIN_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                            break;
                    }
                }

                // 更新 ABUDF_B
                _accessService.AddABUDF_B(form);
                // 更新Form
                _formService.UpdateForm(form);
                // 更新FormB
                _formService.AddFormB(form);
                // 寄送通知
                _formService.SendStatusMail(form);
                // 判斷如果是通過待繳費就產生繳費單(為了先新增ABUDF_1)
                if ((form.VerifyStage1 == VerifyStage.複審通過 && form.FormStatus == FormStatus.通過待繳費) ||
                    (form.VerifyStage2 == VerifyStage.複審通過 && form.CalcStatus == CalcStatus.通過待繳費))
                {
                    string fileName = "";
                    if (form.VerifyStage1 == VerifyStage.複審通過 && form.FormStatus == FormStatus.通過待繳費)
                    {
                        fileName = $"繳款單{form.C_NO}-{form.SER_NO}({(form.P_KIND == "一次繳清" ? "一次繳清" : "第一期")})";
                    }
                    if (form.VerifyStage2 == VerifyStage.複審通過 && form.CalcStatus == CalcStatus.通過待繳費)
                    {
                        fileName = $"繳款單{form.C_NO}-{form.SER_NO}(結算補繳)";
                    }

                    _formService.CreatePaymentPDF(fileName, form);
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 匯出結算退費審核表
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportRefundVerify1(FormView form)
        {
            try
            {
                ChineseMoneyConverter converter = new ChineseMoneyConverter();
                using (var wb = new XLWorkbook(HttpContext.Current.Server.MapPath("~/App_Data/template/RefundVeirfy1Template.xlsx")))
                {
                    var ws = wb.Worksheet(1);
                    ws.Cell("E2").SetValue(DateTime.Now.AddYears(-1911).ToString("yyy年MM月dd日"));
                    ws.Cell("B3").SetValue(form.COMP_NAM);
                    ws.Cell("E3").SetValue($"{form.C_NO}-{form.SER_NO}");
                    ws.Cell("B4").SetValue(form.R_ADDR3);
                    ws.Cell("D4").SetValue(form.B_SERNO);
                    ws.Cell("B5").SetValue(form.S_NAME);
                    ws.Cell("B6").SetValue(form.S_ADDR2);
                    ws.Cell("E6").SetValue(form.S_C_TEL);
                    ws.Cell("B7").SetValue(converter.ToChineseUpper(form.MONEY - form.TAX_MONEY));
                    ws.Cell("B8").SetValue(converter.ToChineseUpper(form.S_AMT.Value));
                    ws.Cell("B9").SetValue(converter.ToChineseUpper(form.S_AMT2.Value));
                    // 已繳空污費金額
                    ws.Cell("B11").SetValue(converter.ToChineseUpper(form.S_AMT.Value));
                    // 溢收總金額
                    double overPayAmount = form.S_AMT.Value > form.S_AMT2.Value ? form.S_AMT.Value - form.S_AMT2.Value : 0;
                    ws.Cell("B12").SetValue(converter.ToChineseUpper(overPayAmount));
                    ws.Cell("B14").SetValue(converter.ToChineseUpper(overPayAmount));

                    // 停工天數
                    double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays + 1);
                    string text1 = overPayAmount > 0 ? "核退" : "核補";
                    string text2 = overPayAmount > 0 ? $"{(form.S_AMT.Value - form.S_AMT2.Value).ToString("N0")}" : $"{(form.S_AMT2.Value - form.S_AMT.Value).ToString("N0")}";
                    string text3 = form.S_AMT2.Value.ToString("N0");
                    string text4 = _formService.GetApplyFormulaText(form, downDays);
                    string text5 = form.S_AMT.Value.ToString("N0");
                    string text6 = _formService.GetCalcFormulaText(form, downDays);
                    string comment = $"一、審核應{text1}： {text2}元\r\n\r\n二、結算申報實際應繳金額：{text3} 元\r\n       計算式：{text4}\r\n\r\n三、未開工前申報應繳金額：{text5} 元\r\n       計算式：{text6}\r\n\r\n四、目前已繳金額(不含逾期利息)：{text5} 元\r\n\r\n五、減免金額：0 元\r\n\r\n六、應{text1}金額：{text2} 元";
                    ws.Cell("B15").SetValue(comment);

                    string fileName = $"{form.C_NO}-{form.SER_NO} 結算退費審核表";
                    string excelPath = HttpContext.Current.Server.MapPath($@"~/App_Data/download/{fileName}.xlsx");
                    wb.SaveAs(excelPath);

                    var stream = new FileStream(excelPath, FileMode.Open);
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    // 傳到前端的檔名
                    // Uri.EscapeDataString 防中文亂碼
                    response.Content.Headers.Add("file-name", Uri.EscapeDataString($"{fileName}.xlsx"));

                    return response;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 匯出結算空污費金額異動原因明細
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public HttpResponseMessage ExportRefundVerify2(FormView form)
        {
            try
            {
                ChineseMoneyConverter converter = new ChineseMoneyConverter();
                using (var wb = new XLWorkbook(HttpContext.Current.Server.MapPath("~/App_Data/template/RefundVeirfy2Template.xlsx")))
                {
                    var ws = wb.Worksheet(1);
                    ws.Cell("B3").SetValue(form.S_AMT == form.FormB.S_AMT ? "無異動" : "有異動");
                    ws.Cell("C3").SetValue(form.S_AMT);
                    ws.Cell("D3").SetValue(form.FormB.S_AMT);

                    ws.Cell("B4").SetValue(form.KIND == form.FormB.KIND ? "無異動" : "有異動");
                    ws.Cell("C4").SetValue(form.KIND);
                    ws.Cell("D4").SetValue(form.FormB.KIND);

                    ws.Cell("B5").SetValue(form.YEAR == form.FormB.YEAR ? "無異動" : "有異動");
                    ws.Cell("C5").SetValue(form.YEAR);
                    ws.Cell("D5").SetValue(form.FormB.YEAR);

                    ws.Cell("B6").SetValue(form.MONEY == form.FormB.MONEY ? "無異動" : "有異動");
                    ws.Cell("C6").SetValue($"{form.MONEY}元");
                    ws.Cell("D6").SetValue($"{form.FormB.MONEY}元");

                    ws.Cell("B7").SetValue(form.AREA == form.FormB.AREA ? "無異動" : "有異動");
                    ws.Cell("C7").SetValue($"{form.AREA}平方公尺");
                    ws.Cell("D7").SetValue($"{form.FormB.AREA}平方公尺");

                    string year = form.B_DATE.Substring(0, 3);
                    string month = form.B_DATE.Substring(3, 2);
                    string day = form.B_DATE.Substring(5, 2);
                    string b_date1 = $"{year}年{month}月{day}日";
                    year = form.FormB.B_DATE.Substring(0, 3);
                    month = form.FormB.B_DATE.Substring(3, 2);
                    day = form.FormB.B_DATE.Substring(5, 2);
                    string b_date2 = $"{year}年{month}月{day}日";

                    year = form.E_DATE.Substring(0, 3);
                    month = form.E_DATE.Substring(3, 2);
                    day = form.E_DATE.Substring(5, 2);
                    string e_date1 = $"{year}年{month}月{day}日";
                    year = form.FormB.E_DATE.Substring(0, 3);
                    month = form.FormB.E_DATE.Substring(3, 2);
                    day = form.FormB.E_DATE.Substring(5, 2);
                    string e_date2 = $"{year}年{month}月{day}日";

                    ws.Cell("B8").SetValue($"{b_date1}至{e_date1}" == $"{b_date2}至{e_date2}" ? "無異動" : "有異動");
                    ws.Cell("C8").SetValue($"{b_date1}至{e_date1}");
                    ws.Cell("D8").SetValue($"{b_date2}至{e_date2}");

                    double downDays = form.StopWorks.Sum(o => o.DOWN_DAY);
                    ws.Cell("B9").SetValue(0 == downDays ? "無停工" : "有停工");
                    ws.Cell("C9").SetValue(0);
                    ws.Cell("D9").SetValue(downDays);

                    int rowIdx = 11;
                    foreach (var item in form.StopWorks)
                    {
                        ws.Cell($"B{rowIdx}").SetValue(item.DOWN_DAY);

                        year = item.DOWN_DATE.Substring(0, 3);
                        month = item.DOWN_DATE.Substring(3, 2);
                        day = item.DOWN_DATE.Substring(5, 2);
                        ws.Cell($"C{rowIdx}").SetValue($"{year}年{month}月{day}日");

                        year = item.UP_DATE.Substring(0, 3);
                        month = item.UP_DATE.Substring(3, 2);
                        day = item.UP_DATE.Substring(5, 2);
                        ws.Cell($"D{rowIdx}").SetValue($"{year}年{month}月{day}日");

                        rowIdx++;
                    }

                    string fileName = $"{form.C_NO}-{form.SER_NO} 結算空污費金額異動原因明細";
                    string excelPath = HttpContext.Current.Server.MapPath($@"~/App_Data/download/{fileName}.xlsx");
                    wb.SaveAs(excelPath);

                    var stream = new FileStream(excelPath, FileMode.Open);
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(stream);
                    response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    // 傳到前端的檔名
                    // Uri.EscapeDataString 防中文亂碼
                    response.Content.Headers.Add("file-name", Uri.EscapeDataString($"{fileName}.xlsx"));

                    return response;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 下載檔案
        /// </summary>
        /// <param name="f">原始檔名</param>
        /// <param name="n">下載檔名</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage Download(string f, string n = null)
        {
            var FilePath = $@"{_uploadPath}\{f}";
            var stream = new FileStream(FilePath, FileMode.Open);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = n ?? f
            };

            return response;
        }

        ///// <summary>
        ///// 下載全部檔案打包壓縮
        ///// </summary>
        ///// <param name="id">申請單ID</param>
        ///// <returns></returns>
        //[HttpGet]
        //public HttpResponseMessage DownloadZip(long id)
        //{
        //    var form = _formService.GetFormByID(id);
        //    var filesToInclude = new List<string>();
        //    foreach (var item in form.Attachments)
        //    {
        //        filesToInclude.Add($@"{_uploadPath}\{item.FileName}");
        //    }

        //    var memoryStream = new MemoryStream();
        //    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        //    {
        //        foreach (var filePath in filesToInclude)
        //        {
        //            var fileName = Path.GetFileName(filePath);
        //            var entry = archive.CreateEntry(fileName);

        //            using (var entryStream = entry.Open())
        //            using (var fileStream = new FileStream(filePath, FileMode.Open))
        //            {
        //                fileStream.CopyTo(entryStream);
        //            }
        //        }
        //    }


        //    memoryStream.Position = 0;
        //    var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
        //    {
        //        Content = new StreamContent(memoryStream)
        //    };
        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        //    {
        //        FileName = $"{form.COMP_NAM}.zip"
        //    };
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/zip");

        //    return result;
        //}
    }
}
