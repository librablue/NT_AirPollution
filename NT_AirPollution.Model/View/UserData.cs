using Newtonsoft.Json;
using NT_AirPollution.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.View
{
    public class UserData
    {
        public long ID { get; set; }
        public UserType UserType { get; set; }
        public string Email { get; set; }
        public string CompanyID { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 管制編號(非會員)
        /// </summary>
        public string C_NO { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public string[] Role { get; set; }
        [JsonIgnore]
        public string ActiveCode { get; set; }
        [JsonIgnore]
        public string Captcha { get; set; }
    }
}
