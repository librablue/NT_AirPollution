﻿using hbehr.recaptcha;
using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NT_AirPollution.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly FormService _formService = new FormService();

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
                if (result == null)
                    throw new Exception("登入失敗，案件編號或 Email 錯誤");

                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                        result.ID.ToString(),
                        DateTime.Now,
                        DateTime.Now.AddHours(8),
                        true,
                        JsonConvert.SerializeObject(new UserData
                        {
                            ID = result.ID,
                            Email = result.CreateUserEmail,
                            AutoFormID = result.AutoFormID,
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

        [Authorize(Roles = "Member,NonMember")]
        public ActionResult Result()
        {
            if (User.IsInRole("NonMember"))
            {
                var fiter = new Form
                {
                    CreateUserEmail = BaseService.CurrentUser.Email,
                    AutoFormID = BaseService.CurrentUser.AutoFormID
                };

                var result = _formService.GetFormByUser(fiter);
                if (result == null)
                    throw new Exception("登入失敗，案件編號或 Email 錯誤");

                return View(result);
            }

            return RedirectToAction("Index");
        }
    }
}