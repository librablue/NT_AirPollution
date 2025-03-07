﻿using Microsoft.SqlServer.Server;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using NT_AirPollution.Service;
using NT_AirPollution.Web.ActionFilter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace NT_AirPollution.Web.Controllers
{
    [CustomAuthorize("Member2")]
    public class RoadController : BaseController
    {
        private readonly RoadService _roadService = new RoadService();
        private readonly FormService _formService = new FormService();

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPromiseByForm(long formID)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(formID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var promise = _roadService.GetPromiseByFormID(formID);
                return Json(new AjaxResult { Status = true, Message = promise }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddPromise(RoadPromiseView promise)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(promise.FormID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var promiseInDB = _roadService.GetPromiseByFormID(promise.FormID);
                if(promiseInDB != null)
                    throw new Exception("禁止重複建立承諾書");

                if (promise.StartDate > promise.EndDate)
                    throw new Exception("起始日期不能大於結束日期");

                promise.FormID = formInDB.ID;
                promise.CreateUserID = BaseService.CurrentUser.ID;
                promise.CreateDate = DateTime.Now;
                _roadService.AddPromise(promise);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }

        public JsonResult GetReportByForm(long formID)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(formID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var report = _roadService.GetReportByFormID(formID);
                return Json(new AjaxResult { Status = true, Message = report }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddReport(RoadReportView view)
        {
            try
            {
                // 檢查是否為自己的申請單
                var formInDB = _formService.GetFormByID(view.FormID);
                if (formInDB == null || (formInDB.S_G_NO != BaseService.CurrentUser.CompanyID && formInDB.R_G_NO != BaseService.CurrentUser.CompanyID))
                    throw new Exception("查無此空污申請單");

                var reportInDB = _roadService.GetReportByFormID(view.FormID);
                bool isExist = reportInDB.Any(o => o.YearMth == view.YearMth);
                if(isExist)
                    throw new Exception("相同年月禁止重複申報");

                var roadReports = new List<RoadReport>();
                foreach (var item in view.Roads)
                {
                    // 自動計算每月清掃長度
                    switch (item.Frequency)
                    {
                        case "每日":
                            item.TotalLength = item.LengthPerTimes * item.Times * 30;
                            break;
                        case "每週":
                            item.TotalLength = item.LengthPerTimes * item.Times * 4;
                            break;
                        default:
                            item.TotalLength = 0;
                            break;
                    }

                    roadReports.Add(new RoadReport
                    {
                        PromiseID = view.PromiseID,
                        RoadID = item.ID,
                        YearMth = view.YearMth,
                        RoadName = item.RoadName,
                        CleanWay1 = item.CleanWay1,
                        CleanWay2 = item.CleanWay2,
                        Frequency = item.Frequency,
                        Times = item.Times,
                        LengthPerTimes = item.LengthPerTimes,
                        TotalLength = item.TotalLength,
                        GreenLength = item.GreenLength,
                        CreateUserID = BaseService.CurrentUser.ID,
                        CreateDate = DateTime.Now
                    });
                }
                _roadService.AddReport(roadReports);
                return Json(new AjaxResult { Status = true });
            }
            catch (Exception ex)
            {
                return Json(new AjaxResult { Status = false, Message = ex.Message });
            }
        }
    }
}