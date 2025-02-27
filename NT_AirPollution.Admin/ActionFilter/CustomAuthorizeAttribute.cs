using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;


namespace NT_AirPollution.Admin.ActionFilter
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] allowedRoles;
        private const string AuthCookieName = "admin"; // 自訂 Cookie 名稱

        public CustomAuthorizeAttribute(params string[] roles)
        {
            this.allowedRoles = roles;
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            // 取得自訂 Cookie
            HttpCookie authCookie = HttpContext.Current.Request.Cookies[AuthCookieName];
            if (authCookie == null)
            {
                return false; // 無 Cookie，驗證失敗
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

            // 取得使用者名稱
            string username = authTicket.Name;
            AdminUser userData = JsonConvert.DeserializeObject<AdminUser>(authTicket.UserData); // 反序列化 UserData
            string[] roles = new string[] { $"{userData.RoleID}" };

            // 設定 HttpContext.User，讓 API Controller 可存取 User.Identity
            HttpContext.Current.User = new GenericPrincipal(new FormsIdentity(authTicket), roles);

            // 如果沒有指定角色，則直接允許
            if (allowedRoles == null || allowedRoles.Length == 0)
            {
                return true;
            }

            // 驗證使用者角色
            bool isValid = roles.Intersect(allowedRoles).Any();
            return isValid;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!IsAuthorized(actionContext))
            {
                // 驗證失敗，回傳 401 未授權
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}