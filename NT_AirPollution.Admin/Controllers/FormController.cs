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
                }

                _formService.UpdateForm(form);
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

                // 更新 Access
                //_accessService.AddABUDF_B(form);
                // 更新表單
                _formService.UpdateForm(form);
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
