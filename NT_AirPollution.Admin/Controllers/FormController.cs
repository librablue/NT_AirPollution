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
        public int GetFinalCalc(FormView form)
        {
            int TotalMoney2 = _formService.CalcTotalMoney(form);
            return TotalMoney2;
        }

        [HttpPost]
        public bool UpdateForm(FormView form)
        {
            try
            {
                _formService.UpdateForm(form);
                _formService.UpdateStopWork(form);
                _formService.UpdatePayment(form);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateFormStatus(FormView form)
        {
            try
            {
                switch (form.FormStatus)
                {
                    case FormStatus.需補件:
                        _formService.SendStatus2(form);
                        break;
                    case FormStatus.通過待繳費:
                        form.TotalMoney1 = _formService.CalcTotalMoney(form);
                        _formService.SendFormStatus3(form);
                        break;
                    case FormStatus.已繳費完成:
                        _formService.SendFormStatus4(form);
                        break;
                }

                form.VerifyDate = DateTime.Now;
                _formService.UpdateForm(form);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool UpdateCalcStatus(FormView form)
        {
            try
            {
                switch (form.CalcStatus)
                {
                    case CalcStatus.需補件:
                        _formService.SendStatus2(form);
                        break;
                    case CalcStatus.通過待繳費:
                        _formService.SendCalcStatus3(form);
                        break;
                    case CalcStatus.通過待退費小於4000:
                    case CalcStatus.通過待退費大於4000:
                        _formService.SendCalcStatus45(form);
                        break;
                    case CalcStatus.繳退費完成:
                        _formService.SendCalcStatus6(form);
                        break;
                    default:
                        break;
                }

                form.VerifyDate = DateTime.Now;
                _formService.UpdateForm(form);
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
