using hbehr.recaptcha;
using Newtonsoft.Json;
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
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;

namespace NT_AirPollution.Web.Controllers
{
    public class MemberController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly ClientUserService _clientUserService = new ClientUserService();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();
        private readonly AccessService _accessService = new AccessService();
        private readonly VerifyService _verifyService = new VerifyService();

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

        [CustomAuthorize(Roles = "Member1,Member2")]
        public ActionResult Edit()
        {
            return View();
        } 

        [HttpPost]
        public JsonResult Login(ClientUser user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.Captcha) || !ReCaptcha.ValidateCaptcha(user.Captcha))
                    throw new Exception("請勾選我不是機器人。");

                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
                    throw new Exception("欄位驗證錯誤。");

                var result = _clientUserService.Login(user);
                if (result == null)
                    throw new Exception("登入失敗，帳號或密碼錯誤。");

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                        result.ID.ToString(),
                        DateTime.Now,
                        DateTime.Now.AddHours(8),
                        true,
                        JsonConvert.SerializeObject(new UserData
                        {
                            ID = result.ID,
                            UserType = result.UserType,
                            Email = result.Email,
                            CompanyID = result.CompanyID,
                            UserName = result.UserName,
                            Role = new string[] { $"Member{(int)result.UserType}" }
                        }),
                        FormsAuthentication.FormsCookiePath);

                string encTicket = FormsAuthentication.Encrypt(ticket);
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                return Json(new AjaxResult { Status = true, Message = result.UserType });
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
                if (sendbox != null && sendbox.CreateDate > DateTime.Now.AddSeconds(-180))
                    throw new Exception("寄送次數太頻繁，請 3 分鐘後再試。");

                // 產生5碼亂數
                string code = "";
                Random rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    code += rnd.Next(0, 9);
                }

                // 驗證信
                string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}/App_Data/Template/VerifyCode.txt");
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
                _verifyService.AddVerifyLog(verify);

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
                if (string.IsNullOrEmpty(user.Captcha) || !ReCaptcha.ValidateCaptcha(user.Captcha))
                    throw new Exception("請勾選我不是機器人。");

                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var existUser = _clientUserService.GetExistClientUser(user.Email);
                if (existUser != null)
                    throw new Exception("相同 Email 已存在。");

                var verifyLog = _verifyService.CheckVerifyLog(user.Email);
                if (verifyLog == null || verifyLog.ActiveCode != user.ActiveCode)
                    throw new Exception("驗證碼錯誤。");

                verifyLog.ActiveDate = DateTime.Now;
                _verifyService.UpdateVerifyLog(verifyLog);

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
                if (sendbox != null && sendbox.CreateDate > DateTime.Now.AddSeconds(-180))
                    throw new Exception("寄送次數太頻繁，請 3 分鐘後再試。");

                // 產生5碼亂數
                string code = "";
                Random rnd = new Random();
                for (int i = 0; i < 5; i++)
                {
                    code += rnd.Next(0, 9);
                }

                // 驗證信
                string template = ($@"{AppDomain.CurrentDomain.BaseDirectory}/App_Data/Template/VerifyCode.txt");
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
                _verifyService.AddVerifyLog(verify);

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
                if (string.IsNullOrEmpty(user.Captcha) || !ReCaptcha.ValidateCaptcha(user.Captcha))
                    throw new Exception("請勾選我不是機器人。");

                ModelState.Remove("UserName");
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var verifyLog = _verifyService.CheckVerifyLog(user.Email);
                if (verifyLog == null || verifyLog.ActiveCode != user.ActiveCode)
                    throw new Exception("驗證碼錯誤。");

                verifyLog.ActiveDate = DateTime.Now;
                _verifyService.UpdateVerifyLog(verifyLog);

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

        [CustomAuthorize(Roles = "Member1,Member2")]
        [HttpPost]
        public JsonResult UpdateProfile(ClientUser user)
        {
            try
            {
                var userInDB = _clientUserService.GetUserByID(BaseService.CurrentUser.ID);
                userInDB.UserName = user.UserName;
                _clientUserService.UpdateClientUser(userInDB);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [CustomAuthorize(Roles = "Member1,Member2")]
        [HttpPost]
        public JsonResult UpdatePassword(ClientUser user)
        {
            try
            {
                var userInDB = _clientUserService.GetUserByID(BaseService.CurrentUser.ID);
                user.Email = userInDB.Email;
                user.CompanyID = userInDB.CompanyID;
                user.CreateDate = userInDB.CreateDate;
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                userInDB.Password = user.Password;
                _clientUserService.UpdateClientUser(userInDB);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [CustomAuthorize(Roles = "Member1,Member2")]
        public JsonResult GetCurrentUser()
        {
            try
            {
                var result = _clientUserService.GetUserByID(BaseService.CurrentUser.ID);
                result.Password = null;
                return Json(new AjaxResult { Status = true, Message = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}