using NT_AirPollution.Model.Domain;
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
    [CustomAuthorize(Roles = "Member2")]
    public class AirController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly AirService _airService = new AirService();
        private readonly FormService _formService = new FormService();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetAirsByForm(int formID)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(formID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var airs = _airService.GetAirsByFormID(formID);
                return Json(new AjaxResult { Status = true, Message = airs }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddAir(AirView air, List<HttpPostedFileBase> file)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(air.FormID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                if (air.StartDate > air.EndDate)
                    throw new Exception("起始日期不能大於結束日期");

                // 設定資料夾
                string absoluteDirPath = $"{_uploadPath}";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                string absoluteFilePath = "";
                if (file != null)
                {
                    foreach (var f in file)
                    {
                        // 生成檔名
                        string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(f.FileName)}";
                        // 設定儲存路徑
                        absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                        // 儲存檔案
                        f.SaveAs(absoluteFilePath);

                        air.AirFiles.Add(new AirFile
                        {
                            FileName = fileName
                        });
                    }
                }

                air.FormID = formInDB.ID;
                air.CreateDate = DateTime.Now;
                air.ModifyDate = DateTime.Now;
                _airService.AddAir(air);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        public JsonResult UpdateAir(AirView air, List<HttpPostedFileBase> file)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(air.FormID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                if (air.StartDate > air.EndDate)
                    throw new Exception("起始日期不能大於結束日期");

                // 設定資料夾
                string absoluteDirPath = $"{_uploadPath}";
                if (!Directory.Exists(absoluteDirPath))
                    Directory.CreateDirectory(absoluteDirPath);

                string absoluteFilePath = "";
                if (file != null)
                {
                    foreach (var f in file)
                    {
                        // 生成檔名
                        string fileName = $@"{Guid.NewGuid().ToString()}{Path.GetExtension(f.FileName)}";
                        // 設定儲存路徑
                        absoluteFilePath = absoluteDirPath + $@"\{fileName}";
                        // 儲存檔案
                        f.SaveAs(absoluteFilePath);

                        air.AirFiles.Add(new AirFile
                        {   AirID = air.ID,
                            FileName = fileName
                        });
                    }
                }

                air.ModifyDate = DateTime.Now;
                _airService.UpdateAir(air);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }

        }

        [HttpPost]
        public JsonResult DeleteAir(AirView air)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(air.FormID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                _airService.DeleteAir(air);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}