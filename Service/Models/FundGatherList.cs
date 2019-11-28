using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Models
{
    [Table("fund_gather_list")]
    public class FundGatherList
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}