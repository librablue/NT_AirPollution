using hbehr.recaptcha;
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

namespace NT_AirPollution.Web.Controllers
{
    public class FormController : BaseController
    {
        private readonly string _uploadPath = ConfigurationManager.AppSettings["UploadPath"].ToString();
        private readonly string _configDomain = ConfigurationManager.AppSettings["Domain"].ToString();
        private readonly FormService _formService = new FormService();
        private readonly OptionService _optionService = new OptionService();
        private readonly SendBoxService _sendBoxService = new SendBoxService();

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
                form.KIND = allProjectCode.First(o => o.Code == form.KIND_NO).Name;
                form.AP_DATE = DateTime.Now.AddYears(-1911).ToString("yyyMMdd");
                form.B_DATE = form.B_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.E_DATE = form.E_DATE2.AddYears(-1911).ToString("yyyMMdd");
                form.S_B_BDATE = form.S_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.R_B_BDATE = form.R_B_BDATE2.AddYears(-1911).ToString("yyyMMdd");
                form.C_DATE = DateTime.Now;
                form.M_DATE = DateTime.Now;
                form.SerialNo = sn + 1;
                form.AutoFormID = DateTime.Now.ToString($"yyyyMMdd{(sn + 1).ToString().PadLeft(3, '0')}");
                form.ActiveCode = Guid.NewGuid().ToString();
                form.IsActive = false;
                form.Status = Status.審理中;
                var id = _formService.AddForm(form);

                // 寄驗證信
                string template = ($@"{HostingEnvironment.ApplicationPhysicalPath}/App_Data/Template/ActiveMail.txt");
                using (StreamReader sr = new StreamReader(template))
                {
                    String content = sr.ReadToEnd();
                    string url = string.Format("{0}/Verify/Index?code={1}", _configDomain, form.ActiveCode);
                    string body = string.Format(content, form.AutoFormID, form.CreateUserEmail, url, url);

                    _sendBoxService.AddSendBox(new SendBox
                    {
                        Address = form.CreateUserEmail,
                        Subject = $"南投縣環保局營建工程空氣污染防制費網路申報系統認證信(案件編號-{form.AutoFormID})",
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
    }
}