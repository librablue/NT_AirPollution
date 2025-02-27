using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Configuration;
using System.Web;
using System.Web.Security;

namespace NT_AirPollution.Service
{
    public class BaseService
    {
        protected readonly string connStr = ConfigurationManager.ConnectionStrings["NT_AirPollution"].ConnectionString;
        protected readonly string accessConnStr = ConfigurationManager.ConnectionStrings["Access"].ConnectionString;
        protected readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        // 台銀繳款單代收類別(固定)
        protected readonly string botCode = "4750";
        public static UserData CurrentUser
        {
            get
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies["member"];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                UserData userData = JsonConvert.DeserializeObject<UserData>(authTicket.UserData);
                return userData;
            }
        }

        public static AdminUser CurrentAdmin
        {
            get
            {
                HttpCookie authCookie = HttpContext.Current.Request.Cookies["admin"];
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                AdminUser userData = JsonConvert.DeserializeObject<AdminUser>(authTicket.UserData);
                return userData;
            }
        }

        /// <summary>
        /// 民國年轉西元年
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DateTime ChineseDateToWestDate(string dt)
        {
            return Convert.ToDateTime($"{Convert.ToInt32(dt.Substring(0, 3)) + 1911}-{dt.Substring(3, 2)}-{dt.Substring(5, 2)}");
        }

        /// <summary>
        /// 物件深拷貝
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T DeepCopy<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
