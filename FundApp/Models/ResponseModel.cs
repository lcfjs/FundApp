using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundApp.Models
{
    /// <summary>
    /// 返回实体
    /// </summary>
    public class ResponseModel
    {
        public object data { get; set; }
        public int code { get; set; }
        public string msg { get; set; }
    }
}
