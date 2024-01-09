using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace NT_AirPollution.Web.ActionFilter
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // 获取用户请求的控制器和操作方法信息
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            // 获取用户的角色信息
            var roles = ((ClaimsIdentity)filterContext.HttpContext.User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Member/Login");
                return;
            }

            if (!string.IsNullOrEmpty(Roles) && !filterContext.HttpContext.User.IsInRole(Roles))
            {
                filterContext.Result = new RedirectResult($"~/Member/Login?ReturnUrl=%2f{controllerName}%2f{actionName}");
                return;
            }
        }
    }
}