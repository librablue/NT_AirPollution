using hbehr.recaptcha;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class FormController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly ClientUserService _clientUserService = new ClientUserService();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();
        private readonly AccessService _accessService = new AccessService();
        private readonly VerifyService _verifyService = new VerifyService();

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Calc()
        {
            return View();
        }

        public ActionResult ProjectTable()
        {
            return View();
        }

        public JsonResult GetTotalMoney(FormView form)
        {
            try
            {
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                var result = _formService.CalcTotalMoney(form, 0);
                return Json(new AjaxResult { Status = true, Message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = "計算過程發生異常" }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpPost]
        //public JsonResult Create(FormView form, List<HttpPostedFileBase> files)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(form.Captcha) || !ReCaptcha.ValidateCaptcha(form.Captcha))
        //            throw new Exception("請勾選我不是機器人");

        //        if (!ModelState.IsValid)
        //            throw new Exception("欄位驗證錯誤");

        //        if (form.B_DATE2 > form.E_DATE2)
        //            throw new Exception("施工期程起始日期不能大於結束日期");

        //        var verifyLog = _verifyService.CheckVerifyLog(form.CreateUserEmail);
        //        if (verifyLog == null || verifyLog.ActiveCode != form.ActiveCode)
        //            throw new Exception("電子信箱驗證碼錯誤");


        //        var allDists = _optionService.GetDistrict();
        //        var allProjectCode = _optionService.GetProjectCode();
        //        form.SER_NO = 1;
        //        form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
        //        form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
        //        form.AP_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
        //        form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
        //        form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
        //        form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
        //        form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
        //        form.C_DATE = DateTime.Now;
        //        form.M_DATE = DateTime.Now;
        //        form.IsActive = false;
        //        form.FormStatus = FormStatus.審理中;
        //        form.CalcStatus = CalcStatus.未申請;
        //        string c_no = _accessService.GetC_NO(form);
        //        form.C_NO = c_no;

        //        // 寫入 Access
        //        _accessService.AddABUDF(form);

        //        _formService.AddForm(form);
        //        verifyLog.ActiveDate = DateTime.Now;
        //        _verifyService.UpdateVerifyLog(verifyLog);

        //        // 寄驗證信
        //        string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/CreateFormMail.txt");
        //        using (StreamReader sr = new StreamReader(template))
        //        {
        //            String content = sr.ReadToEnd();
        //            string body = string.Format(content, form.C_NO, form.CreateUserEmail);

        //            _sendBoxService.AddSendBox(new SendBox
        //            {
        //                Address = form.CreateUserEmail,
        //                Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統認證信(管制編號-{form.C_NO})",
        //                Body = body,
        //                FailTimes = 0,
        //                CreateDate = DateTime.Now
        //            });
        //        }

        //        return Json(new AjaxResult { Status = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new AjaxResult { Status = false, Message = ex.Message });
        //    }
        //}

        [HttpPost]
        public JsonResult SendActiveCode(VerifyLog verify)
        {
            try
            {
                var sendbox = _sendBoxService.CheckSendBoxFrequency(verify.Email);
                if (sendbox != null && sendbox.CreateDate > DateTime.Now.AddSeconds(-180))
                    throw new Exception("寄送次數太頻繁，請 3 分鐘後再試。");

                // 產生5碼亂數
                string code = "";
                Random rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    code += rnd.Next(0, 9);
                }

                // 寄驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/VerifyCode.txt");
                using (StreamReader sr = new StreamReader(template))
                {
                    String content = sr.ReadToEnd();
                    string body = string.Format(content, code);

                    _sendBoxService.AddSendBox(new SendBox
                    {
                        Address = verify.Email,
                        Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統認證信",
                        Body = body,
                        FailTimes = 0,
                        CreateDate = DateTime.Now
                    });
                }

                var verifyInDB = _verifyService.CheckVerifyLog(verify.Email);
                // 控制DB一個email只存一次
                if (verifyInDB == null)
                {
                    verifyInDB = new VerifyLog
                    {
                        Email = verify.Email,
                        ActiveCode= code,
                        CreateDate= DateTime.Now
                    };
                    _verifyService.AddVerifyLog(verifyInDB);
                }
                else
                {
                    verifyInDB.ActiveCode = code;
                    verifyInDB.CreateDate = DateTime.Now;
                    _verifyService.UpdateVerifyLog(verifyInDB);
                }
                    
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}