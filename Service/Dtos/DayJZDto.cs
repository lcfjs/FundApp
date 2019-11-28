using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Dtos
{
    /// <summary>
    /// 每日净值
    /// </summary>
    public class DayJZDto
    {
        public DateTime FSRQ { get; set; }
        public decimal DWJZ { get; set; }
        public decimal LJJZ { get; set; }
        public decimal? JZZZL { get; set; }
        public string SDATE { get; set; }
        public string SGZT { get; set; }
        public string SHZT { get; set; }
    }
}
