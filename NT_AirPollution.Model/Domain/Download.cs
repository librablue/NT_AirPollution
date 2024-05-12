using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Download")]
    public class Download
    {
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Filename { get; set; }
        [Required]
        public int Sort { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime? DeleteDate { get; set; }
    }
}
