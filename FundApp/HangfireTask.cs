using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundApp
{
    public class HangfireTask
    {
        private static object lockObj = new object();

        private static HangfireTask _instance;

        public static HangfireTask Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new HangfireTask();
                        }
                    }
                }
                return _instance;
            }
        }

        private IServiceCollection _services;

        public IServiceCollection Services
        {
            get
            {
                return _services;
            }
            set
            {
                _services = value;
                serviceProvider = _services.BuildServiceProvider();
            }
        }

        private static ServiceProvider serviceProvider = null; //或使用静态，使用时则不需调用实例

        public void AddTasks()
        {
            //0 0 2 1/1 * ? 每天凌晨两点执行一次
            RecurringJob.AddOrUpdate(() => RunGetDayData(), "0 0 2 * * ?", TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate(() => RunGetMinData(), "* 0/1 9-15 ? * 1,2,3,4,5", TimeZoneInfo.Local);
        }

        /// <summary>
        /// 采集每日数据
        /// </summary>
        public void RunGetDayData()
        {
            var service = serviceProvider.GetService<FundDataDayService>();
            var gatherList = serviceProvider.GetService<FundCodeService>().GetGatherList();
            if (gatherList.Count > 0)
            {
                foreach (var item in gatherList)
                {
                    service.GetDataTask(item.Code);
                }
            }
            else
            {
                service.GetDataTask();
            }
        }

        /// <summary>
        /// 采集每日分钟数据
        /// </summary>
        public void RunGetMinData()
        {
            var service = serviceProvider.GetService<FundDataMinService>();
            service.GetDataTask();
        }
    }
}
