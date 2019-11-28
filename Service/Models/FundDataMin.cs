using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Models
{
    /// <summary>
    /// 每分钟数据
    /// </summary>
    [Table("fund_data_min")]
    public class FundDataMin
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 昨日净值
        /// </summary>
        public decimal Zrjz { get; set; }
        /// <summary>
        /// 净值时间
        /// </summary>
        public DateTime Jzsj { get; set; }
        /// <summary>
        /// 估算值
        /// </summary>
        public decimal Gsz { get; set; }
        /// <summary>
        /// 估算收益
        /// </summary>
        public decimal Gssy { get; set; }
    }
}
