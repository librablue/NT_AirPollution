﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    public class UserData
    {
        public long ID { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// 案件編號
        /// </summary>
        public string AutoFormID { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string[] Role { get; set; }
        public string ActiveCode { get; set; }
        public string Captcha { get; set; }
    }
}