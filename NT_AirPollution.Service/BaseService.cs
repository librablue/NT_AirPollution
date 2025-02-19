﻿using Dapper;
using Newtonsoft.Json;
using NT_AirPollution.Model.Domain;
using NT_AirPollution.Model.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    UserData clientUser = JsonConvert.DeserializeObject<UserData>(ticket.UserData);
                    return clientUser;
                }

                return null;
            }
        }

        public static AdminUser CurrentAdmin
        {
            get
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                    FormsAuthenticationTicket ticket = id.Ticket;
                    AdminUser user = JsonConvert.DeserializeObject<AdminUser>(ticket.UserData);
                    return user;
                }

                return null;
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
