using hbehr.recaptcha;
using Newtonsoft.Json;
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
using System.Web.Security;

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

        public ActionResult Regist()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Forget()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Edit()
        {
            return View();
        }

        [Authorize]
        public ActionResult Company()
        {
            return View();
        }

        [Authorize]
        public ActionResult Contractor()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Login(ClientUser user)
        {
            try
            {
                if (user.Captcha == null || !ReCaptcha.ValidateCaptcha(user.Captcha))
                    throw new Exception("請勾選我不是機器人。");

                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                    throw new Exception("欄位驗證錯誤。");

                var result = _clientUserService.Login(user);
                if (result == null)
                    throw new Exception("登入失敗，帳號或密碼錯誤。");

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                        result.Email,
                        DateTime.Now,
                        DateTime.Now.AddHours(8),
                        true,
                        JsonConvert.SerializeObject(new UserData
                        {
                            ID = result.ID,
                            Email = result.Email
                        }),
                        FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SendRegistCode(VerifyLog verify)
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

                var sendbox = _sendBoxService.CheckSendBoxFrequency(verify.Email);
                if (sendbox != null && sendbox.CreateDate > DateTime.Now.AddMinutes(-3))
                    throw new Exception("寄送次數太頻繁，請 3 分鐘後再試。");

                // 產生5碼亂數
                string code = "";
                Random rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    code += rnd.Next(0, 9);
                }

                // 驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/VerifyCode.txt");
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

                return Json(new AjaxResult { Status = true, Message = "已發送驗證碼到您申請的信箱，請在5分鐘內收取驗證碼進行驗證。" });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Regist(ClientUser user)
        {
            try
            {
                if (user.Captcha == null || !ReCaptcha.ValidateCaptcha(user.Captcha))
                    throw new Exception("請勾選我不是機器人。");

                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var existUser = _clientUserService.GetExistClientUser(user.Email);
                if (existUser != null)
                    throw new Exception("相同 Email 已存在。");

                var verifyLog = _clientUserService.CheckVerifyLog(user.Email);
                if (verifyLog == null || verifyLog.ActiveCode != user.ActiveCode)
                    throw new Exception("驗證碼錯誤。");

                verifyLog.ActiveDate = DateTime.Now;
                _clientUserService.UpdateVerifyLog(verifyLog);

                user.CreateDate = DateTime.Now;
                _clientUserService.AddUser(user);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SendForgetCode(VerifyLog verify)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var existUser = _clientUserService.GetExistClientUser(verify.Email);
                if (existUser == null)
                    throw new Exception("查無此 Email 帳號。");

                var sendbox = _sendBoxService.CheckSendBoxFrequency(verify.Email);
                if (sendbox != null && sendbox.CreateDate > DateTime.Now.AddMinutes(-3))
                    throw new Exception("寄送次數太頻繁，請 3 分鐘後再試。");

                // 產生5碼亂數
                string code = "";
                Random rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    code += rnd.Next(0, 9);
                }

                // 驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/VerifyCode.txt");
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

                return Json(new AjaxResult { Status = true, Message = "已發送驗證碼到您申請的信箱，請在5分鐘內收取驗證碼進行驗證。" });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Forget(ClientUser user)
        {
            try
            {
                if (user.Captcha == null || !ReCaptcha.ValidateCaptcha(user.Captcha))
                    throw new Exception("請勾選我不是機器人。");

                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var verifyLog = _clientUserService.CheckVerifyLog(user.Email);
                if (verifyLog == null || verifyLog.ActiveCode != user.ActiveCode)
                    throw new Exception("驗證碼錯誤。");

                verifyLog.ActiveDate = DateTime.Now;
                _clientUserService.UpdateVerifyLog(verifyLog);

                var userInDB = _clientUserService.GetUserByEmail(user.Email);
                if (userInDB == null)
                    throw new Exception("查無此 Email 帳號。");

                userInDB.Password = user.Password;
                _clientUserService.UpdateClientUser(userInDB);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult Edit(ClientUser user)
        {
            try
            {
                user.Email = BaseService.CurrentUser.Email;
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                _clientUserService.UpdateClientUser(user);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetMyCompanies(ClientUserCompany filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _clientUserService.GetCompanyByUser(filter);
            return Json(result);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddCompany(ClientUserCompany company)
        {
            try
            {
                company.ClientUserID = BaseService.CurrentUser.ID;
                company.S_B_ID = company.S_B_ID.ToUpper();
                company.S_C_ID = company.S_C_ID.ToUpper();
                company.S_B_BDATE = company.S_B_BDATE2?.AddYears(-1911).ToString("yyyMMdd");
                company.CreateDate = DateTime.Now;
                company.ModifyDate = DateTime.Now;
                _clientUserService.AddCompany(company);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult UpdateCompany(ClientUserCompany company)
        {
            try
            {
                // 檢查是否為真實的資料
                var companyInDB = _clientUserService.GetCompanyByID(company.ID);
                if (companyInDB == null || companyInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("查無資料。");

                company.ClientUserID = BaseService.CurrentUser.ID;
                company.S_B_ID = company.S_B_ID.ToUpper();
                company.S_C_ID = company.S_C_ID.ToUpper();
                company.S_B_BDATE = company.S_B_BDATE2?.AddYears(-1911).ToString("yyyMMdd");
                company.ModifyDate = DateTime.Now;
                _clientUserService.UpdateCompany(company);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteCompany(ClientUserCompany company)
        {
            try
            {
                // 檢查是否為真實的資料
                var companyInDB = _clientUserService.GetCompanyByID(company.ID);
                if (companyInDB == null || companyInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("查無資料。");

                company.ClientUserID = BaseService.CurrentUser.ID;
                _clientUserService.DeleteCompany(company);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public JsonResult GetGetMyContractor(ClientUserContractor filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _clientUserService.GetContractorByUser(filter);
            return Json(result);
        }
    }
}