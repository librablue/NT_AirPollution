using Dapper.Contrib.Extensions;
using NT_AirPollution.Model.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("ClientUser")]
    public class ClientUser
    {
        [Dapper.Contrib.Extensions.Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Email 格式錯誤")]
        [EmailAddress(ErrorMessage = "Email 格式錯誤")]
        [MaxLength(50, ErrorMessage = "Email 超出長度")]
        public string Email { get; set; }
        [Required(ErrorMessage = "未選擇會員類別")]
        public UserType UserType { get; set; }
        [Required(ErrorMessage = "密碼格式錯誤")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$", ErrorMessage = "密碼格式錯誤")]
        public string Password { get; set; }
        [Required(ErrorMessage = "未輸入統一編號")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "統一編號格式錯誤")]
        public string CompanyID { get; set; }
        [Required(ErrorMessage = "未輸入姓名")]
        [MaxLength(20, ErrorMessage = "姓名超出長度")]
        public string UserName { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        [Computed]
        public string ActiveCode { get; set; }
        [Computed]
        public string Captcha { get; set; }
    }
}
