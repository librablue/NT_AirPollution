﻿using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.Model.Domain
{
    [Table("Payment")]
    public class Payment
    {
        [Key]
        public long ID { get; set; }
        public long FormID { get; set; }
        public int Amount { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
