using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FundApp.Models;
using Service.Services;

namespace FundApp.Controllers
{
    public class HomeController : BaseController
    {
        private readonly FundCodeService fundCodeService;
        private readonly FundDataDayService fundDataDayService;
        private readonly FundDataMinService fundDataMinService;
        public HomeController(FundCodeService fundCodeService, FundDataDayService fundDataDayService, FundDataMinService fundDataMinService)
        {
            this.fundCodeService = fundCodeService;
            this.fundDataDayService = fundDataDayService;
            this.fundDataMinService = fundDataMinService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Privacy";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




        public IActionResult Insert()
        {
            fundCodeService.Insert();
            return Json(Success());
        }

        public IActionResult GetGatherList()
        {
            var list = fundCodeService.GetGatherList();
            return Json(Success(list));
        }

        public IActionResult AddGather(string code)
        {
            var msg = fundCodeService.AddGatherCode(code);
            return Json(Success(msg));
        }

        public IActionResult GetMinChartData(string fundCode)
        {
            return Json(Success(fundDataMinService.GetChartData(fundCode)));
        }

        public IActionResult GetDayChartData(string fundCode, DateTime? date1, DateTime? date2)
        {
            return Json(Success(fundDataDayService.GetChartData(fundCode, date1, date2)));
        }

        public IActionResult GetAllChartData(string fundCodes, DateTime? date1, DateTime? date2)
        {
            var fundCodeArr = fundCodes.Split(",", StringSplitOptions.RemoveEmptyEntries);
            return Json(Success(fundDataDayService.GetAllChartData(fundCodeArr, date1, date2)));
        }

        public IActionResult GetDayTableData(string fundCode, DateTime? date1, DateTime? date2, int pageIndex, int pageSize)
        {
            int total = 0;
            if (date1 == null) date1 = DateTime.Now.Date.AddMonths(-1);
            if (date2 == null) date2 = DateTime.Now.Date;
            return Json(Success(new PageModel { Data = fundDataDayService.GetPageList(fundCode, date1, date2, out total, pageIndex, pageSize), Total = total }));
        }
    }
}
