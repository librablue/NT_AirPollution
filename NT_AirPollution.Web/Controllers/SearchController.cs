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
    public class SearchController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();
        private readonly AccessService _accessService = new AccessService();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(FormView form)
        {
            try
            {
                if (string.IsNullOrEmpty(form.Captcha) || !ReCaptcha.ValidateCaptcha(form.Captcha))
                    throw new Exception("請勾選我不是機器人");

                var result = _formService.GetFormByUser(form);
                if (result.Count() == 0)
                    throw new Exception("登入失敗，案件編號或 Email 錯誤");

                var firstResult = result.First();
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                        firstResult.ID.ToString(),
                        DateTime.Now,
                        DateTime.Now.AddHours(8),
                        true,
                        JsonConvert.SerializeObject(new UserData
                        {
                            ID = firstResult.ID,
                            Email = firstResult.CreateUserEmail,
                            C_NO = firstResult.C_NO,
                            Role = new string[] { "NonMember" }
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

        [CustomAuthorize(Roles = "NonMember")]
        public ActionResult Result()
        {
            return View();
        }

        [CustomAuthorize(Roles = "NonMember")]
        public JsonResult GetMyForm()
        {
            var fiter = new Form
            {
                CreateUserEmail = BaseService.CurrentUser.Email,
                C_NO = BaseService.CurrentUser.C_NO
            };
            var result = _formService.GetFormByUser(fiter);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize(Roles = "NonMember")]
        [HttpPost]
        public JsonResult UpdateForm(FormView form, List<HttpPostedFileBase> files)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var filter = new Form
                {
                    CreateUserEmail = BaseService.CurrentUser.Email,
                    C_NO = BaseService.CurrentUser.C_NO
                };
                var formInDB = _formService.GetFormByUser(filter).FirstOrDefault(o => o.SER_NO == form.SER_NO);
                if (formInDB == null)
                    throw new Exception("申請單不存在");

                if (form.B_DATE2 > form.E_DATE2)
                    throw new Exception("施工期程起始日期不能大於結束日期");

                var info = _optionService.GetAttachmentInfo().Where(o => o.PUB_COMP == form.PUB_COMP).ToList();
                if (info.Count() != files.Count() || form.Attachments.Count() != info.Count())
                    throw new Exception("檔案上傳數量異常");


                List<string> allowExt = new List<string> { ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png" };
                foreach (var file in files)
                {
                    if (file == null || file.ContentLength == 0)
                        continue;

                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowExt.Any(o => o == ext))
                        throw new Exception("附件只允許上傳 doc/docx/pdf/jpg/png 等文件");
                }

                // 設定資料夾
                string absoluteDirPath = $"{_uploadPath}";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                string absoluteFilePath = "";
                int i = 0;
                foreach (var file in files)
                {
                    if (file == null || file.ContentLength == 0)
                    {
                        i++;
                        continue;
                    }

                    // 生成檔名
                    string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                    // 設定儲存路徑
                    absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                    // 儲存檔案
                    file.SaveAs(absoluteFilePath);
                    form.Attachments[i].InfoID = i + 1;
                    form.Attachments[i].FileName = fileName;
                    i++;
                }

                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();
                // 避免被修改的欄位
                form.C_NO = formInDB.C_NO;
                form.SER_NO = formInDB.SER_NO;
                form.ClientUserID = formInDB.ClientUserID;
                form.CreateUserEmail = formInDB.CreateUserEmail;
                form.ActiveCode = formInDB.ActiveCode;
                form.IsActive = formInDB.IsActive;
                form.S_AMT = formInDB.S_AMT;
                form.S_AMT2 = formInDB.S_AMT2;
                form.C_DATE = formInDB.C_DATE;
                form.VerifyDate = formInDB.VerifyDate;
                form.FailReason = formInDB.FailReason;
                // 可修改的欄位
                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
                form.AP_DATE = formInDB.AP_DATE;
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.M_DATE = DateTime.Now;
                form.FormStatus = FormStatus.審理中;
                form.CalcStatus = CalcStatus.未申請;

                // 修改 access
                bool isAccessOK = _accessService.UpdateABUDF(form);
                if (!isAccessOK)
                    throw new Exception("系統發生未預期錯誤");

                _formService.UpdateForm(form);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}