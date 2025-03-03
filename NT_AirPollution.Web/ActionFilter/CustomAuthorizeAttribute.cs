using Newtonsoft.Json;
using NT_AirPollution.Model.View;
using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NT_AirPollution.Web.ActionFilter
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;
        private const string AuthCookieName = "member"; // 自訂 Cookie 名稱

        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            // 取得自訂 Cookie
            HttpCookie authCookie = httpContext.Request.Cookies[AuthCookieName];
            if (authCookie == null)
            {
                return false; // 沒有 Cookie，視為未登入
            }

            // 嘗試解密 Cookie
            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return false; // Cookie 解密失敗
            }

            if (authTicket == null || authTicket.Expired)
            {
                return false; // Ticket 無效或過期
            }

            UserData userData = JsonConvert.DeserializeObject<UserData>(authTicket.UserData); // 反序列化 UserData

            // 設定 HttpContext 以便 `HandleUnauthorizedRequest` 可以讀取
            httpContext.Items["AuthUser"] = authTicket.Name;
            httpContext.Items["AuthRoles"] = authTicket.UserData;

            // 設定 HttpContext.User 讓後續授權驗證可以使用
            httpContext.User = new GenericPrincipal(new FormsIdentity(authTicket), userData.Role);

            // 如果沒有指定角色，則直接允許
            if (allowedRoles == null || allowedRoles.Length == 0)
            {
                return true;
            }

            // 驗證使用者角色
            bool isValid = userData.Role.Intersect(allowedRoles).Any();
            return isValid;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            try
            {
                // 未登入
                if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                {
                    filterContext.Result = new RedirectResult("~/Member/Login");
                    return;
                }

                string username = (string)filterContext.HttpContext.Items["AuthUser"]?.ToString();
                UserData userData = JsonConvert.DeserializeObject<UserData>(filterContext.HttpContext.Items["AuthRoles"]?.ToString());

                // 获取用户请求的控制器和操作方法信息
                var controllerName = filterContext.RouteData.Values["controller"].ToString();
                var actionName = filterContext.RouteData.Values["action"].ToString();

                // 角色不允許
                if (!userData.Role.Intersect(allowedRoles).Any())
                {
                    filterContext.Result = new RedirectResult($"~/Member/Login?ReturnUrl=%2f{controllerName}%2f{actionName}");
                    return;
                }
            }
            catch (Exception ex)
            {
                filterContext.Result = new RedirectResult("~/Member/Login");
            }
        }
    }
}