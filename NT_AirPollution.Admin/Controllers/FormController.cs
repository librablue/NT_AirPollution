using NT_AirPollution.Admin.ActionFilter;
using NT_AirPollution.Model.Domain;
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
    [AuthorizeUser]
    public class FormController : ApiController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly FormService _formService = new FormService();
        private readonly AccessService _accessService = new AccessService();

        [HttpPost]
        public List<FormView> GetForms(FormFilter filter)
        {
            var forms = _formService.GetForms(filter);
            return forms;
        }

        /// <summary>
        /// 結算金額
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public double GetFinalCalc(FormView form)
        {
            form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
            form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
            // 停工天數
            double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays + 1);
            var result = _formService.CalcTotalMoney(form, downDays);
            return result.TotalMoney;
        }

        [HttpPost]
        public bool AddForm(FormView form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                if (form.B_DATE2 > form.E_DATE2)
                    throw new Exception("施工期程起始日期不能大於結束日期");

                form.SER_NO += 1;
                form.AP_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.C_DATE = DateTime.Now;
                form.M_DATE = DateTime.Now;
                form.FormStatus = FormStatus.審理中;
                form.CalcStatus = CalcStatus.未申請;

                //// 20240516 改審核後才產生單號
                //// 寫入 Access
                //bool isAccessOK = _accessService.AddABUDF(form);
                //if (!isAccessOK)
                //    throw new Exception("系統發生未預期錯誤");

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
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");

                // 有管制編號才更新Access
                if (!string.IsNullOrEmpty(form.C_NO))
                {
                    // 修改 access
                    bool isAccessOK = _accessService.UpdateABUDF(form);
                    if (!isAccessOK)
                        throw new Exception("更新 Access 發生未預期錯誤");
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
                bool isAccessOK = _accessService.AddABUDF(form);
                if (!isAccessOK)
                    throw new Exception("系統發生未預期錯誤");

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
                switch (form.FormStatus)
                {
                    case FormStatus.待補件:
                        break;
                    case FormStatus.通過待繳費:
                        form.VerifyDate1 = DateTime.Now;
                        // 停工天數
                        double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays + 1);
                        var result = _formService.CalcTotalMoney(form, downDays);
                        form.S_AMT = result.TotalMoney;
                        form.P_NUM = form.P_KIND == "一次全繳" ? 1 : 2;
                        form.P_AMT = form.S_AMT;
                        if (form.P_KIND == "分兩次繳清")
                            form.P_AMT = form.S_AMT / 2;


                        if (form.S_AMT <= 100)
                            form.FormStatus = FormStatus.免繳費;

                        break;
                    case FormStatus.已繳費完成:
                        break;
                }

                // 3.4.5指令共用，用退費金額<4000判斷4，>=4000判斷5
                if (form.CalcStatus == CalcStatus.通過待繳費)
                {
                    form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                    form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                    // 停工天數
                    double downDays = form.StopWorks.Sum(o => (o.UP_DATE2 - o.DOWN_DATE2).TotalDays + 1);
                    var result = _formService.CalcTotalMoney(form, downDays);
                    form.S_AMT2 = result.TotalMoney;

                    if (form.S_AMT2 > form.S_AMT)
                        form.CalcStatus = CalcStatus.通過待繳費;
                    else if (form.P_AMT == form.S_AMT2)
                        form.CalcStatus = CalcStatus.繳退費完成;
                    else if (form.P_AMT - form.S_AMT2 < 4000)
                        form.CalcStatus = CalcStatus.通過待退費小於4000;
                    else if (form.P_AMT - form.S_AMT2 >= 4000)
                        form.CalcStatus = CalcStatus.通過待退費大於4000;
                }

                switch (form.CalcStatus)
                {
                    case CalcStatus.待補件:
                        break;
                    case CalcStatus.通過待繳費:
                        form.VerifyDate2 = DateTime.Now;
                        break;
                    case CalcStatus.通過待退費小於4000:
                    case CalcStatus.通過待退費大於4000:
                        break;
                    case CalcStatus.繳退費完成:
                        var isAccessOK = _accessService.AddABUDF_B(form);
                        if (!isAccessOK)
                            throw new Exception("更新 Access 發生未預期錯誤");

                        form.FIN_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                        break;
                }

                _formService.UpdateForm(form);
                _formService.SendStatusMail(form);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public HttpResponseMessage Download(string f)
        {
            var FilePath = $@"{_uploadPath}\{f}";
            var stream = new FileStream(FilePath, FileMode.Open);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(stream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = f
            };

            return response;
        }
    }
}
