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

        public JsonResult GetTotalMoney(FormView form)
        {
            form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
            form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
            double S_AMT = _formService.CalcTotalMoney(form, 0);
            return Json(new AjaxResult { Status = true, Message = S_AMT }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Create(FormView form, List<HttpPostedFileBase> files)
        {
            try
            {
                if (string.IsNullOrEmpty(form.Captcha) || !ReCaptcha.ValidateCaptcha(form.Captcha))
                    throw new Exception("請勾選我不是機器人");

                if (!ModelState.IsValid)
                    throw new Exception("欄位驗證錯誤");

                if (form.B_DATE2 > form.E_DATE2)
                    throw new Exception("施工期程起始日期不能大於結束日期");

                var verifyLog = _verifyService.CheckVerifyLog(form.CreateUserEmail);
                if (verifyLog == null || verifyLog.ActiveCode != form.ActiveCode)
                    throw new Exception("電子信箱驗證碼錯誤");


                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();
                form.SER_NO = 1;
                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
                form.AP_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.C_DATE = DateTime.Now;
                form.M_DATE = DateTime.Now;
                form.IsActive = false;
                form.FormStatus = FormStatus.審理中;
                form.CalcStatus = CalcStatus.未申請;
                string c_no = _accessService.GetC_NO(form);
                form.C_NO = c_no;

                // 寫入 Access
                _accessService.AddABUDF(form);

                _formService.AddForm(form);
                verifyLog.ActiveDate = DateTime.Now;
                _verifyService.UpdateVerifyLog(verifyLog);

                // 寄驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/CreateFormMail.txt");
                using (StreamReader sr = new StreamReader(template))
                {
                    String content = sr.ReadToEnd();
                    string body = string.Format(content, form.C_NO, form.CreateUserEmail);

                    _sendBoxService.AddSendBox(new SendBox
                    {
                        Address = form.CreateUserEmail,
                        Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統認證信(管制編號-{form.C_NO})",
                        Body = body,
                        FailTimes = 0,
                        CreateDate = DateTime.Now
                    });
                }

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

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
                    string body = string.Format(content, verify.ActiveCode);

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

        /// <summary>
        /// 上傳附件
        /// </summary>
        [HttpPost]
        public JsonResult UploadAttachment(HttpPostedFileBase file)
        {
            try
            {
                if (file == null) throw new Exception("請選擇檔案");
                // 設定資料夾
                string absoluteDirPath = $"{_uploadPath}";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                string absoluteFilePath = "";
                List<string> allowExt = new List<string> { ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png" };
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (!allowExt.Any(o => o == ext))
                    throw new Exception("附件只允許上傳 doc/docx/pdf/jpg/png 等文件");

                if (file.ContentLength >= 1024 * 1024 * 4)
                    throw new Exception("附件大小限制 4MB");

                // 生成檔名
                string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                // 設定儲存路徑
                absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                // 儲存檔案
                file.SaveAs(absoluteFilePath);

                return Json(new AjaxResult { Status = true, Message = fileName });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 下載繳費單
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize(Roles = "Member1,NonMember")]
        [HttpPost]
        public FileResult DownloadPayment(FormView form)
        {
            var formInDB = _formService.GetFormByID(form.ID);
            if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                throw new Exception("申請單不存在");

            string fileName = $"繳款單{form.C_NO}-{form.SER_NO}({(form.P_KIND == "一次繳清" ? "一次繳清" : "第一期")})";
            string pdfPath = _formService.CreatePaymentPDF(fileName, form);

            // 傳到前端的檔名
            // Uri.EscapeDataString 防中文亂碼
            Response.Headers.Add("file-name", Uri.EscapeDataString(Path.GetFileName(pdfPath)));

            return File(pdfPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pdfPath));
        }

        /// <summary>
        /// 結算申請
        /// </summary>
        [CustomAuthorize(Roles = "Member1,NonMember")]
        [HttpPost]
        public JsonResult FinalCalc(FormView form)
        {
            try
            {
                var formInDB = _formService.GetFormByID(form.ID);
                if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                    throw new Exception("申請單不存在");

                formInDB.AP_DATE1 = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                formInDB.CalcStatus = CalcStatus.審理中;
                _formService.UpdateForm(formInDB);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 下載補繳費單
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize(Roles = "Member1,NonMember")]
        [HttpPost]
        public FileResult DownloadRePayment(FormView form)
        {
            var formInDB = _formService.GetFormByID(form.ID);
            if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                throw new Exception("申請單不存在");

            string fileName = $"繳款單{form.C_NO}-{form.SER_NO}(結算補繳)";
            string pdfPath = _formService.CreatePaymentPDF(fileName, form);

            // 傳到前端的檔名
            // Uri.EscapeDataString 防中文亂碼
            Response.Headers.Add("file-name", Uri.EscapeDataString(Path.GetFileName(pdfPath)));

            return File(pdfPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pdfPath));
        }

        /// <summary>
        /// 新增退款帳戶
        /// </summary>
        [CustomAuthorize(Roles = "Member1,NonMember")]
        [HttpPost]
        public JsonResult UpdateBankAccount(RefundBank bank, HttpPostedFileBase file)
        {
            try
            {
                var formInDB = _formService.GetFormByID(bank.FormID);
                if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                    throw new Exception("申請單不存在");

                if (formInDB.CalcStatus == CalcStatus.繳退費完成)
                    throw new Exception("申請單已繳退費完成，無法修改帳戶");


                if (file != null)
                {
                    // 設定資料夾
                    string absoluteDirPath = $"{_uploadPath}";
                    if (!Directory.Exists(absoluteDirPath))
                        Directory.CreateDirectory(absoluteDirPath);

                    string absoluteFilePath = "";
                    List<string> allowExt = new List<string> { ".jpg", ".jpeg", ".png" };
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowExt.Any(o => o == ext))
                        throw new Exception("附件只允許上傳 jpg/png 等文件");

                    if (file.ContentLength >= 1024 * 1024 * 4)
                        throw new Exception("附件大小限制 4MB");

                    // 生成檔名
                    string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                    // 設定儲存路徑
                    absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                    // 儲存檔案
                    file.SaveAs(absoluteFilePath);
                    bank.Photo = fileName;
                }

                bank.FormID = formInDB.ID;
                bank.CreateDate = DateTime.Now;
                _formService.UpdateRefundBank(bank);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 下載補繳費單
        /// </summary>
        /// <returns></returns>
        [CustomAuthorize(Roles = "Member1,NonMember")]
        [HttpPost]
        public FileResult DownloadProof(FormView form)
        {
            var formInDB = _formService.GetFormByID(form.ID);
            if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                throw new Exception("申請單不存在");

            string pdfPath = _formService.CreateProofPDF(form);

            // 傳到前端的檔名
            // Uri.EscapeDataString 防中文亂碼
            Response.Headers.Add("file-name", Uri.EscapeDataString(Path.GetFileName(pdfPath)));

            return File(pdfPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pdfPath));
        }

        /// <summary>
        /// 上傳繳費證明
        /// </summary>
        [CustomAuthorize(Roles = "Member1,NonMember")]
        [HttpPost]
        public JsonResult UploadPaymentProof(PaymentProof proof, HttpPostedFileBase file)
        {
            try
            {
                var formInDB = _formService.GetFormByID(proof.FormID);
                if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                    throw new Exception("申請單不存在");

                if (formInDB.CalcStatus == CalcStatus.繳退費完成)
                    throw new Exception("申請單已繳退費完成，無法修改帳戶");


                if (file != null)
                {
                    // 設定資料夾
                    string absoluteDirPath = $"{_uploadPath}";
                    if (!Directory.Exists(absoluteDirPath))
                        Directory.CreateDirectory(absoluteDirPath);

                    string absoluteFilePath = "";
                    List<string> allowExt = new List<string> { ".jpg", ".jpeg", ".png" };
                    string ext = Path.GetExtension(file.FileName).ToLower();
                    if (!allowExt.Any(o => o == ext))
                        throw new Exception("附件只允許上傳 jpg/png 等文件");

                    if (file.ContentLength >= 1024 * 1024 * 4)
                        throw new Exception("附件大小限制 4MB");

                    // 生成檔名
                    string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                    // 設定儲存路徑
                    absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                    // 儲存檔案
                    file.SaveAs(absoluteFilePath);
                    proof.ProofFile = fileName;
                }

                proof.CreateDate = DateTime.Now;
                _formService.UpdatePaymentProof(proof);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}