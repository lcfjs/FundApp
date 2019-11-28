using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Models
{
    /// <summary>
    /// 每日数据
    /// </summary>
    [Table("fund_data_day")]
    public class FundDataDay
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
        /// <summary>
        /// 净值日期
        /// </summary>
        public DateTime Jzrq { get; set; }
        /// <summary>
        /// 净值
        /// </summary>
        public decimal Jz { get; set; }
        /// <summary>
        /// 累计净值
        /// </summary>
        public decimal Ljjz { get; set; }
        /// <summary>
        /// 净值增长率
        /// </summary>
        public decimal? JZZZL { get; set; }
    }
}
