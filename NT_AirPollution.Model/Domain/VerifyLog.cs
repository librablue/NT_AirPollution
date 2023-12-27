using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("VerifyLog")]
    public class VerifyLog
    {
        [Dapper.Contrib.Extensions.Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Email 格式錯誤")]
        [EmailAddress(ErrorMessage = "Email 格式錯誤")]
        public string Email { get; set; }
        public string ActiveCode { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
