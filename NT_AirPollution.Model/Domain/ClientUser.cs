using Dapper.Contrib.Extensions;
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
        [MaxLength(50, ErrorMessage = "Email 格式錯誤")]
        public string Email { get; set; }
        [Required(ErrorMessage = "密碼格式錯誤")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{6,20}$", ErrorMessage = "密碼格式錯誤")]
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        [Computed]
        public string ActiveCode { get; set; }
        [Computed]
        public string Captcha { get; set; }
    }
}
