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

        [HttpPost]
        public JsonResult AddForm(FormView form, List<HttpPostedFileBase> files)
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
                form.ClientUserID = BaseService.CurrentUser.ID;
                form.FormStatus = FormStatus.審理中;
                form.CalcStatus = CalcStatus.未申請;

                // 如果管制編號有值，表示用複製的
                if (string.IsNullOrEmpty(form.C_NO))
                {
                    string c_no = _accessService.GetC_NO(form);
                    form.C_NO = c_no;
                }
                else
                {
                    var filter = new FormFilter { C_NO = form.C_NO, ClientUserID = BaseService.CurrentUser.ID };
                    var formsInDB = _formService.GetFormsByC_NO(filter);
                    if (formsInDB.Count() == 0)
                        throw new Exception("查無複製的管制編號");

                    // 相同管制編號的序號最大值+1
                    form.SER_NO = formsInDB.Last().SER_NO + 1;
                }

                // 寫入 Access
                bool isAccessOK = _accessService.AddABUDF(form);
                if (!isAccessOK)
                    throw new Exception("系統發生未預期錯誤");

                _formService.AddForm(form);

                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

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

                var formInDB = _formService.GetFormByID(form.ID);
                if (formInDB.ClientUserID != BaseService.CurrentUser.ID)
                    throw new Exception("無法修改他人申請單");

                if (form.B_DATE2 > form.E_DATE2)
                    throw new Exception("施工期程起始日期不能大於結束日期");

                var info = _optionService.GetAttachmentInfo().Where(o => o.PUB_COMP == form.PUB_COMP).ToList();
                if(info.Count() != files.Count() || form.Attachments.Count() != info.Count())
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
                form.TotalMoney1 = formInDB.TotalMoney1;
                form.TotalMoney2 = formInDB.TotalMoney2;
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