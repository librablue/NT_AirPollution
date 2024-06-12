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
        public JsonResult AddForm(FormView form)
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
                form.FormStatus = FormStatus.未申請;
                form.CalcStatus = CalcStatus.未申請;
                //// 20240516 改審核後才產生單號
                //string c_no = _accessService.GetC_NO(form);
                //form.C_NO = c_no;

                //// 寫入 Access
                //bool isAccessOK = _accessService.AddABUDF(form);
                //if (!isAccessOK)
                //    throw new Exception("系統發生未預期錯誤");

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

                if (form.B_DATE2 > form.E_DATE2)
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
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.M_DATE = DateTime.Now;

                // 有管制編號才修改Access
                if (!string.IsNullOrEmpty(form.C_NO))
                {
                    bool isAccessOK = _accessService.UpdateABUDF(form);
                    if (!isAccessOK)
                        throw new Exception("系統發生未預期錯誤");
                }

                _formService.UpdateForm(form);

                return Json(new AjaxResult { Status = true });
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
    }
}