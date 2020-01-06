using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FundApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace FundApp.Controllers
{
    public class BaseController : Controller
    {
        protected ResponseModel Success(object data, string msg = "操作成功")
        {
            return new ResponseModel { code = 0, msg = msg, data = data };
        }
        protected ResponseModel Success()
        {
            return new ResponseModel { code = 0, msg = "操作成功", data = null };
        }
    }
}
