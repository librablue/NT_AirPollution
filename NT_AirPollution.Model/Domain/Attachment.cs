using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Attachment")]
    public class Attachment
    {
        [Computed]
        public object this[string propertyName]
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }

        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public long InfoID { get; set; }
        public string FileName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
