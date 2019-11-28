using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Models
{
    [Table("fund_code")]
    public class FundCode
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string PinYin { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Extend1 { get; set; }
        public string Extend2 { get; set; }
        public string Extend3 { get; set; }
        public string Remark { get; set; }
    }
}