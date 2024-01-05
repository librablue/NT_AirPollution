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

        [HttpPost]
        public bool UpdateForm(FormView form)
        {
            try
            {
                var formInDB = _formService.GetFormByID(form.ID);
                if (form.Status != formInDB.Status)
                {
                    switch (form.Status)
                    {
                        case Status.需補件:
                            _formService.SendStatus2(form);
                            break;
                        case Status.通過待繳費:
                            form.VerifyDate = DateTime.Now;
                            _formService.SendStatus3(form);
                            break;
                        case Status.已繳費完成:
                            _formService.SendStatus4(form);
                            break;
                    }
                }

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
            var FilePath = $@"{_uploadPath}/{f}";
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
