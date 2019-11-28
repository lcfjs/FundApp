using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundApp.Models
{
    /// <summary>
    /// 分页实体
    /// </summary>
    public class PageModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public object Data { get; set; }
    }
}
