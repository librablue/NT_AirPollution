using hbehr.recaptcha;
using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using static System.Net.WebRequestMethods;

namespace NT_AirPollution.Web.Controllers
{
    public class MemberController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly ClientUserService _clientUserService = new ClientUserService();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
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

        [Authorize(Roles = "Member")]
        public ActionResult Edit()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public ActionResult Company()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public ActionResult Contractor()
        {
            return View();
        }

        [Authorize(Roles = "Member")]
        public ActionResult Form()
        {
            ViewBag.CurrentUser = _clientUserService.GetUserByID(BaseService.CurrentUser.ID);
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
                            Email = result.Email,
                            Role = new string[] { "Member" }
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
                if (string.IsNullOrEmpty(user.Captcha) || !ReCaptcha.ValidateCaptcha(user.Captcha))
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

        [Authorize(Roles = "Member")]
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

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult GetMyCompanies(ClientUserCompany filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _clientUserService.GetCompanyByUser(filter);
            return Json(result);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult AddCompany(ClientUserCompany company)
        {
            try
            {
                company.ClientUserID = BaseService.CurrentUser.ID;
                company.S_B_ID = company.S_B_ID?.ToUpper();
                company.S_C_ID = company.S_C_ID?.ToUpper();
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

        [Authorize(Roles = "Member")]
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
                company.S_B_ID = company.S_B_ID?.ToUpper();
                company.S_C_ID = company.S_C_ID?.ToUpper();
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

        [Authorize(Roles = "Member")]
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

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult GetMyContractor(ClientUserContractor filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _clientUserService.GetContractorByUser(filter);
            return Json(result);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult AddContractor(ClientUserContractor contractor)
        {
            try
            {
                contractor.ClientUserID = BaseService.CurrentUser.ID;
                contractor.R_B_ID = contractor.R_B_ID?.ToUpper();
                contractor.R_B_BDATE = contractor.R_B_BDATE2?.AddYears(-1911).ToString("yyyMMdd");
                contractor.CreateDate = DateTime.Now;
                contractor.ModifyDate = DateTime.Now;
                _clientUserService.AddContractor(contractor);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult UpdateContractor(ClientUserContractor contractor)
        {
            try
            {
                // 檢查是否為真實的資料
                var contractorInDB = _clientUserService.GetContractorByID(contractor.ID);
                if (contractorInDB == null || contractorInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("查無資料。");

                contractor.ClientUserID = BaseService.CurrentUser.ID;
                contractor.R_B_ID = contractor.R_B_ID?.ToUpper();
                contractor.R_B_BDATE = contractor.R_B_BDATE2?.AddYears(-1911).ToString("yyyMMdd");
                contractor.ModifyDate = DateTime.Now;
                _clientUserService.UpdateContractor(contractor);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult DeleteContractor(ClientUserContractor contractor)
        {
            try
            {
                // 檢查是否為真實的資料
                var contractorInDB = _clientUserService.GetContractorByID(contractor.ID);
                if (contractorInDB == null || contractorInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("查無資料。");

                contractor.ClientUserID = BaseService.CurrentUser.ID;
                _clientUserService.DeleteContractor(contractor);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult GetForms(FormFilter filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _formService.GetFormsByUser(filter);
            return Json(result);
        }

        [Authorize(Roles = "Member")]
        [HttpPost]
        public JsonResult AddForm(FormView form, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5, HttpPostedFileBase file6, HttpPostedFileBase file7, HttpPostedFileBase file8)
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

                // 如果管制編號有值，表示用複製的
                if (!string.IsNullOrEmpty(form.C_NO))
                {
                    var filter = new FormFilter { C_NO = form.C_NO, ClientUserID = BaseService.CurrentUser.ID };
                    var formsInDB = _formService.GetFormsByC_NO(filter);
                    if (formsInDB.Count() == 0)
                        throw new Exception("查無所複製的管制編號");

                    // 相同管制編號的序號最大值+1
                    form.SER_NO = formsInDB.Last().SER_NO + 1;
                }

                //if (file1 == null)
                //    throw new Exception("未上傳空氣污染防制費申報表");
                //if (file2 == null)
                //    throw new Exception("未上傳建築執照影印本");
                //if (file3 == null)
                //    throw new Exception("未上傳營建業主身分證影本");
                //if (file4 == null)
                //    throw new Exception("未上傳簡易位置圖");
                //if (file5 == null)
                //    throw new Exception("未上傳承包商營利事業登記證");
                //if (file6 == null)
                //    throw new Exception("未上傳承包商負責人身分證影本");

                var attachFile = new AttachmentFile();
                attachFile.File1 = file1;
                attachFile.File2 = file2;
                attachFile.File3 = file3;
                attachFile.File4 = file4;
                attachFile.File5 = file5;
                attachFile.File6 = file6;
                attachFile.File7 = file7;
                attachFile.File8 = file8;


                // 設定資料夾
                string absoluteDirPath = $"{_uploadPath}";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                string absoluteFilePath = "";

                for (int i = 1; i <= 8; i++)
                {
                    var file = (HttpPostedFileBase)attachFile[$"File{i}"];
                    if (file != null)
                    {
                        // 生成檔名
                        string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                        // 設定儲存路徑
                        absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                        // 儲存檔案
                        file.SaveAs(absoluteFilePath);
                        form.Attachment[$"File{i}"] = fileName;
                    }
                }

                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();
                var sn = _formService.GetSerialNumber();

                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
                form.AP_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.C_DATE = DateTime.Now;
                form.M_DATE = DateTime.Now;
                form.SerialNo = sn + 1;
                form.AutoFormID = DateTime.Now.ToString($"yyyyMMdd{(sn + 1).ToString().PadLeft(3, '0')}");
                form.ClientUserID = BaseService.CurrentUser.ID;
                form.Status = Status.審理中;
                _formService.AddForm(form);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}