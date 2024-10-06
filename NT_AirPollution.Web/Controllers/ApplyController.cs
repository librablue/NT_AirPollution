using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.Enum;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize(Roles = "Member1")]
    public class ApplyController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _paymentPath = ConfigurationManager.AppSettings["Payment"].ToString();
        private readonly ClientUserService _clientUserService = new ClientUserService();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();
        private readonly AccessService _accessService = new AccessService();

        public ActionResult Company()
        {
            return View();
        }

        public ActionResult Contractor()
        {
            return View();
        }

        public ActionResult Form()
        {
            ViewBag.CurrentUser = _clientUserService.GetUserByID(BaseService.CurrentUser.ID);
            return View();
        }

        [HttpPost]
        public JsonResult GetMyCompanies(ClientUserCompany filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _clientUserService.GetCompanyByUser(filter);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddCompany(ClientUserCompany company)
        {
            try
            {
                company.ClientUserID = BaseService.CurrentUser.ID;
                company.S_B_ID = company.S_B_ID?.ToUpper();
                company.S_C_ID = company.S_C_ID?.ToUpper();

                // 用統編找現有資料
                var filter = new ClientUserCompany
                {
                    S_G_NO = company.S_G_NO,
                    ClientUserID = BaseService.CurrentUser.ID
                };
                var companyInDB = _clientUserService.GetCompanyByUser(filter);
                if (companyInDB.Count() == 0)
                {
                    company.CreateDate = DateTime.Now;
                    company.ModifyDate = DateTime.Now;
                    _clientUserService.AddCompany(company);
                }
                else
                {
                    var firstCompanyInDB = companyInDB.First();
                    company.ID = firstCompanyInDB.ID;
                    company.CreateDate = firstCompanyInDB.CreateDate;
                    company.ModifyDate = DateTime.Now;
                    _clientUserService.UpdateCompany(company);
                }

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

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
                company.ModifyDate = DateTime.Now;
                _clientUserService.UpdateCompany(company);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

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

        [HttpPost]
        public JsonResult GetMyContractor(ClientUserContractor filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _clientUserService.GetContractorByUser(filter);
            return Json(result);
        }

        [HttpPost]
        public JsonResult AddContractor(ClientUserContractor contractor)
        {
            try
            {
                contractor.ClientUserID = BaseService.CurrentUser.ID;
                contractor.R_B_ID = contractor.R_B_ID?.ToUpper();

                // 用統編找現有資料
                var filter = new ClientUserContractor
                {
                    R_G_NO = contractor.R_G_NO,
                    ClientUserID = BaseService.CurrentUser.ID
                };
                var contractorInDB = _clientUserService.GetContractorByUser(filter);
                if (contractorInDB.Count() == 0)
                {
                    contractor.CreateDate = DateTime.Now;
                    contractor.ModifyDate = DateTime.Now;
                    _clientUserService.AddContractor(contractor);
                }
                else
                {
                    var firstCompanyInDB = contractorInDB.First();
                    contractor.ID = firstCompanyInDB.ID;
                    contractor.CreateDate = firstCompanyInDB.CreateDate;
                    contractor.ModifyDate = DateTime.Now;
                    _clientUserService.UpdateContractor(contractor);
                }

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

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
                contractor.ModifyDate = DateTime.Now;
                _clientUserService.UpdateContractor(contractor);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

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

        [HttpPost]
        public JsonResult GetForms(FormFilter filter)
        {
            filter.ClientUserID = BaseService.CurrentUser.ID;
            var result = _formService.GetFormsByUser(filter);
            return Json(result);
        }

        public JsonResult GetEmptyFormModel()
        {
            FormView form = new FormView();
            return Json(new AjaxResult { Status = true, Message = form }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddForm(FormView form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                if (_formService.ChineseDateToWestDate(form.B_DATE) > _formService.ChineseDateToWestDate(form.E_DATE))
                    throw new Exception("施工期程起始日期不能大於結束日期");

                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();
                form.SER_NO = 1;
                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
                form.AP_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                form.C_DATE = DateTime.Now;
                form.M_DATE = DateTime.Now;
                form.ClientUserID = BaseService.CurrentUser.ID;
                form.FormStatus = FormStatus.未申請;
                form.CalcStatus = CalcStatus.未申請;
                
                if (form.KIND_NO == "1" || form.KIND_NO == "2")
                {
                    // 1、2類工程面積=建築面積
                    form.AREA = form.AREA_B;
                    // 建蔽率=(建築面積AREA_B)/(基地面積AREA_F)*100%
                    form.PERC_B = Math.Round((double)(form.AREA_B / form.AREA_F * 100), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    form.AREA_F = null;
                    form.AREA_B = null;
                    form.PERC_B = null;
                }

                //// 20240516 改審核後才產生單號
                //string c_no = _accessService.GetC_NO(form);
                //form.C_NO = c_no;

                //// 寫入 Access
                //_accessService.AddABUDF(form);

                _formService.AddForm(form);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateForm(FormView form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var formInDB = _formService.GetFormByID(form.ID);
                if (formInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("無法修改他人申請單");

                if (_formService.ChineseDateToWestDate(form.B_DATE) > _formService.ChineseDateToWestDate(form.E_DATE))
                    throw new Exception("施工期程起始日期不能大於結束日期");


                var allDists = _optionService.GetDistrict();
                var allProjectCode = _optionService.GetProjectCode();
                // 避免被修改的欄位
                form.C_NO = formInDB.C_NO;
                form.SER_NO = formInDB.SER_NO;
                form.AP_DATE = formInDB.AP_DATE;
                form.AP_DATE1 = formInDB.AP_DATE1;
                form.ClientUserID = formInDB.ClientUserID;
                form.CreateUserEmail = formInDB.CreateUserEmail;
                form.P_AMT = formInDB.P_AMT;
                form.S_AMT = formInDB.S_AMT;
                form.S_AMT2 = formInDB.S_AMT2;
                form.C_DATE = formInDB.C_DATE;
                form.VerifyDate1 = formInDB.VerifyDate1;
                form.VerifyDate2 = formInDB.VerifyDate2;
                form.FailReason1 = formInDB.FailReason1;
                form.FailReason2 = formInDB.FailReason2;
                form.FormStatus = formInDB.FormStatus;
                form.CalcStatus = formInDB.CalcStatus;
                // 可修改的欄位
                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
                form.AP_DATE = formInDB.AP_DATE;
                form.M_DATE = DateTime.Now;

                if (form.KIND_NO == "1" || form.KIND_NO == "2")
                {
                    // 1、2類工程面積=建築面積
                    form.AREA = form.AREA_B;
                    // 建蔽率=(建築面積AREA_B)/(基地面積AREA_F)*100%
                    form.PERC_B = Math.Round((double)(form.AREA_B / form.AREA_F * 100), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    form.AREA_F = null;
                    form.AREA_B = null;
                    form.PERC_B = null;
                }


                // 有管制編號才修改Access
                if (!string.IsNullOrEmpty(form.C_NO))
                {
                    _accessService.UpdateABUDF(form);
                }

                _formService.UpdateForm(form);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteForm(FormView form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var formInDB = _formService.GetFormByID(form.ID);
                if (formInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("無法刪除他人申請單");

                if(!string.IsNullOrEmpty(formInDB.C_NO))
                    throw new Exception("申請單已審核無法刪除");

                _formService.DeleteForm(form);

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
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file == null) throw new Exception("請選擇檔案");
                // 設定資料夾
                string absoluteDirPath = $"{_uploadPath}";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                string absoluteFilePath = "";
                List<string> allowExt = new List<string> { ".pdf" };
                string ext = Path.GetExtension(file.FileName).ToLower();
                if (!allowExt.Any(o => o == ext))
                    throw new Exception("附件只允許上傳 pdf 文件");

                if (file.ContentLength > 1024 * 1024 * 100)
                    throw new Exception("附件大小限制 100MB");

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
        /// 審核申請
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendFormStatus1(FormView form)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    string firstError = ModelState.Values.SelectMany(o => o.Errors).First().ErrorMessage;
                    throw new Exception(firstError);
                }

                var formInDB = _formService.GetFormByID(form.ID);
                if (formInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("無法修改他人申請單");

                formInDB.M_DATE = DateTime.Now;
                formInDB.FormStatus = FormStatus.審理中;
                _formService.UpdateForm(formInDB);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        /// <summary>
        /// 結算申請
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SendCalcStatus1(FormView form)
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
        /// 下載繳費單
        /// </summary>
        /// <returns></returns>
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
        /// 下載補繳費單
        /// </summary>
        /// <returns></returns>
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
        /// 下載結清證明
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileResult DownloadClearProof(FormView form)
        {
            var formInDB = _formService.GetFormByID(form.ID);
            if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                throw new Exception("申請單不存在");

            string pdfPath = _formService.CreateClearProofPDF(form);

            // 傳到前端的檔名
            // Uri.EscapeDataString 防中文亂碼
            Response.Headers.Add("file-name", Uri.EscapeDataString(Path.GetFileName(pdfPath)));

            return File(pdfPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pdfPath));
        }

        /// <summary>
        /// 下載免徵證明
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileResult DownloadFreeProof(FormView form)
        {
            var formInDB = _formService.GetFormByID(form.ID);
            if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                throw new Exception("申請單不存在");

            string pdfPath = _formService.CreateFreeProofPDF(form);

            // 傳到前端的檔名
            // Uri.EscapeDataString 防中文亂碼
            Response.Headers.Add("file-name", Uri.EscapeDataString(Path.GetFileName(pdfPath)));

            return File(pdfPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pdfPath));
        }

        /// <summary>
        /// 下載繳費證明
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileResult DownloadPaymentProof(FormView form)
        {
            var formInDB = _formService.GetFormByID(form.ID);
            if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
                throw new Exception("申請單不存在");

            string pdfPath = _formService.CreatePaymentProofPDF(form);

            // 傳到前端的檔名
            // Uri.EscapeDataString 防中文亂碼
            Response.Headers.Add("file-name", Uri.EscapeDataString(Path.GetFileName(pdfPath)));

            return File(pdfPath, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(pdfPath));
        }

        ///// <summary>
        ///// 上傳繳費證明
        ///// </summary>
        //[HttpPost]
        //public JsonResult UploadPaymentProof(PaymentProof proof, HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        var formInDB = _formService.GetFormByID(proof.FormID);
        //        if (formInDB == null || (formInDB.ClientUserID != BaseService.CurrentUser.ID && formInDB.CreateUserEmail != BaseService.CurrentUser.Email))
        //            throw new Exception("申請單不存在");

        //        if (formInDB.CalcStatus == CalcStatus.繳退費完成)
        //            throw new Exception("申請單已繳退費完成，無法修改帳戶");


        //        if (file != null)
        //        {
        //            // 設定資料夾
        //            string absoluteDirPath = $"{_uploadPath}";
        //            if (!Directory.Exists(absoluteDirPath))
        //                Directory.CreateDirectory(absoluteDirPath);

        //            string absoluteFilePath = "";
        //            List<string> allowExt = new List<string> { ".jpg", ".jpeg", ".png" };
        //            string ext = Path.GetExtension(file.FileName).ToLower();
        //            if (!allowExt.Any(o => o == ext))
        //                throw new Exception("附件只允許上傳 jpg/png 等文件");

        //            if (file.ContentLength >= 1024 * 1024 * 4)
        //                throw new Exception("附件大小限制 4MB");

        //            // 生成檔名
        //            string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(file.FileName)}";
        //            // 設定儲存路徑
        //            absoluteFilePath = absoluteDirPath + $@"\{fileName}";
        //            // 儲存檔案
        //            file.SaveAs(absoluteFilePath);
        //            proof.ProofFile = fileName;
        //        }

        //        proof.CreateDate = DateTime.Now;
        //        _formService.UpdatePaymentProof(proof);

        //        return Json(new AjaxResult { Status = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new AjaxResult { Status = false, Message = ex.Message });
        //    }
        //}
    }
}