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
using System.Web.Hosting;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class FormController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();
        private readonly AccessService _accessService = new AccessService();

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Create(FormView form, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5, HttpPostedFileBase file6, HttpPostedFileBase file7, HttpPostedFileBase file8)
        {
            try
            {
                if (string.IsNullOrEmpty(form.Captcha) || !ReCaptcha.ValidateCaptcha(form.Captcha))
                    throw new Exception("請勾選我不是機器人");

                if (!ModelState.IsValid)
                    throw new Exception("欄位驗證錯誤");

                if (form.B_DATE2 > form.E_DATE2)
                    throw new Exception("施工期程起始日期不能大於結束日期");


                var attachFile = new AttachmentFile();
                attachFile.File1 = file1;
                attachFile.File2 = file2;
                attachFile.File3 = file3;
                attachFile.File4 = file4;
                attachFile.File5 = file5;
                attachFile.File6 = file6;
                attachFile.File7 = file7;
                attachFile.File8 = file8;

                List<string> allowExt = new List<string> { ".doc", ".docx", ".pdf", ".jpg", ".jpeg", ".png" };
                for (int i = 1; i <= 8; i++)
                {
                    var file = (HttpPostedFileBase)attachFile[$"File{i}"];
                    if (file == null)
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
                for (int i = 1; i <= 8; i++)
                {
                    var file = (HttpPostedFileBase)attachFile[$"File{i}"];
                    if (file == null)
                        continue;

                    // 生成檔名
                    string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
                    // 設定儲存路徑
                    absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                    // 儲存檔案
                    file.SaveAs(absoluteFilePath);
                    form.Attachment[$"File{i}"] = fileName;
                }

                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();
                //var sn = _formService.GetSerialNumber();
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
                form.ActiveCode = Guid.NewGuid().ToString();
                form.IsActive = false;
                form.FormStatus = FormStatus.審理中;
                form.CalcStatus = CalcStatus.未申請;
                string c_no = _accessService.GetC_NO(form);
                form.C_NO = c_no;

                // 寫入 Access
                bool isAccessOK = _accessService.AddABUDF(form);
                if (!isAccessOK)
                    throw new Exception("系統發生未預期錯誤");

                _formService.AddForm(form);

                // 寄驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/ActiveMail.txt");
                using (StreamReader sr = new StreamReader(template))
                {
                    String content = sr.ReadToEnd();
                    string url = string.Format("{0}/Verify/Index?code={1}", _configDomain, form.ActiveCode);
                    string body = string.Format(content, form.C_NO, form.CreateUserEmail, url, url);

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

            int payableAmount = form.TotalMoney1;
            if (form.P_KIND == "分兩次繳清")
                payableAmount = form.TotalMoney1 / 2;

            string bankAccount = _formService.GetBankAccount(form.ID.ToString(), payableAmount);
            string postAccount = _formService.GetPostAccount(form.ID.ToString(), form.TotalMoney1);
            string fileName = $"繳款單{form.C_NO}-{form.SER_NO}({(form.P_KIND == "一次繳清" ? "一次繳清" : "第一期")}).pdf";
            string pdfPath = _formService.CreatePaymentPDF(bankAccount, postAccount, fileName, form);

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

            int payableAmount = form.TotalMoney2 - form.TotalMoney1;
            string bankAccount = _formService.GetBankAccount(form.ID.ToString(), payableAmount);
            string postAccount = _formService.GetPostAccount(form.ID.ToString(), form.TotalMoney1);
            string fileName = $"繳款單{form.C_NO}-{form.SER_NO}(補繳).pdf";
            string pdfPath = _formService.CreatePaymentPDF(bankAccount, postAccount, fileName, form);

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

            string fileName = $"結清證明{form.C_NO}-{form.SER_NO}.pdf";
            string pdfPath = _formService.CreateProofPDF(fileName, form);

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