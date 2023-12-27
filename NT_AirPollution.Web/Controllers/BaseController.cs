using NT_AirPollution.Web.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// 覆寫JSON方法
        /// 修正原本日期格式問題
        /// </summary>
        /// <param name="data"></param>
        /// <param name="contentType"></param>
        /// <param name="contentEncoding"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            // 模擬原本阻擋 GET 的機制
            if (behavior == JsonRequestBehavior.DenyGet &&
                string.Equals(this.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult();
            }

            return new JsonNetResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding
            };
        }
    }
}