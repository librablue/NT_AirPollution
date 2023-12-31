﻿using NT_AirPollution.Model.Domain;
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
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize(Roles = "Member1")]
    public class ApplyController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
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
                form.Status = Status.審理中;

                // 停復工
                foreach (var item in form.StopWorks)
                {
                    item.DOWN_DATE = item.DOWN_DATE2.AddYears(-1911).ToString("yyyMMdd");
                    item.UP_DATE = item.UP_DATE2.AddYears(-1911).ToString("yyyMMdd");
                    item.DOWN_DAY = Convert.ToInt32((item.UP_DATE2 - item.DOWN_DATE2).TotalDays + 1);
                    item.C_DATE = DateTime.Now;
                    item.M_DATE = DateTime.Now;
                }

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

                return Json(new AjaxResult { Status = true, Message = 0 });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateForm(FormView form, HttpPostedFileBase file1, HttpPostedFileBase file2, HttpPostedFileBase file3, HttpPostedFileBase file4, HttpPostedFileBase file5, HttpPostedFileBase file6, HttpPostedFileBase file7, HttpPostedFileBase file8)
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
                // 避免被修改的欄位
                form.C_NO = formInDB.C_NO;
                form.SER_NO = formInDB.SER_NO;
                form.ClientUserID = formInDB.ClientUserID;
                form.CreateUserEmail = formInDB.CreateUserEmail;
                form.ActiveCode = formInDB.ActiveCode;
                form.IsActive = formInDB.IsActive;
                form.C_DATE = formInDB.C_DATE;
                // 可修改的欄位
                form.TOWN_NA = allDists.First(o => o.Code == form.TOWN_NO).Name;
                form.KIND = allProjectCode.First(o => o.ID == form.KIND_NO).Name;
                form.AP_DATE = formInDB.AP_DATE;
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.M_DATE = DateTime.Now;
                form.Status = Status.審理中;

                // 停復工
                foreach (var item in form.StopWorks)
                {
                    item.FormID = form.ID;
                    item.DOWN_DATE = item.DOWN_DATE2.AddYears(-1911).ToString("yyyMMdd");
                    item.UP_DATE = item.UP_DATE2.AddYears(-1911).ToString("yyyMMdd");
                    item.DOWN_DAY = Convert.ToInt32((item.UP_DATE2 - item.DOWN_DATE2).TotalDays + 1);
                    item.C_DATE = DateTime.Now;
                    item.M_DATE = DateTime.Now;
                }

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