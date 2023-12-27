using NT_AirPollution.Model.Domain;
using NT_AirPollution.Service;
using NT_AirPollution.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class MemberController : BaseController
    {
        private readonly ClientUserService _clientUserService = new ClientUserService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignIn()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Forget()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SendCode(VerifyLog verify)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var existUser = _clientUserService.GetExistClientUser(verify.Email);
                if (existUser != null)
                    throw new Exception("相同 Email 已存在。");

                // 產生5碼亂數
                string code = ""; ;
                Random rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    code += rnd.Next(0, 9);
                }

                // 驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/MailTemplate/VerifyCode.txt");
                using (StreamReader sr = new StreamReader(template))
                {
                    string content = sr.ReadToEnd();
                    string body = string.Format(content, code);
                    _sendBoxService.AddSendBox(new SendBox
                    {
                        Address = verify.Email,
                        Subject = "南投縣環保局營建工程空氣污染防制費網路申報系統 - Email驗證碼",
                        Body = body,
                        FailTimes = 0,
                        CreateDate = DateTime.Now
                    });
                }

                verify.ActiveCode = code;
                verify.CreateDate = DateTime.Now;
                _clientUserService.AddVerifyLog(verify);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}