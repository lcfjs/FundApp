using Newtonsoft.Json;
using RestSharp;
using Service.Dtos;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FundDataDayService
    {
        private readonly DapperHelper dapperHelper = null;

        public FundDataDayService(DapperHelper dapperHelper)
        {
            this.dapperHelper = dapperHelper;
        }

        public List<FundDataDay> GetPageList(string fundCode, DateTime? startDate, DateTime? endDate, out int total, int pageIndex = 0, int pageSize = 20)
        {
            DateTime startTime = DateTime.Now.Date.AddDays(-30);
            DateTime endTime = DateTime.Now;
            if (startDate != null) startTime = startDate.Value;
            if (endDate != null) endTime = endDate.Value.AddDays(1).AddMilliseconds(-1);

            var sql = @"SELECT COUNT(1) FROM fund_data_day WHERE jzrq >= @startTime AND jzrq <= @endTime AND code=@code;
                        SELECT * FROM fund_data_day WHERE jzrq >= @startTime AND jzrq <= @endTime AND code=@code ORDER BY jzrq DESC LIMIT @pageIndex,@pageSize";
            var totalCount = 0;
            List<FundDataDay> list = null;
            dapperHelper.QueryMultiple(sql, (reader) =>
            {
                totalCount = reader.ReadFirst<int>();
                list = reader.Read<FundDataDay>().ToList();
            }, new { code = fundCode, startTime, endTime, pageIndex = pageIndex * pageSize, pageSize });
            total = totalCount;
            return list;
        }

        public object GetChartData(string fundCode, DateTime? startDate, DateTime? endDate)
        {
            DateTime startTime = DateTime.Now.Date.AddDays(-30);
            DateTime endTime = DateTime.Now;
            if (startDate != null) startTime = startDate.Value;
            if (endDate != null) endTime = endDate.Value.AddDays(1).AddMilliseconds(-1);

            var list = dapperHelper.GetList<FundDataDay>(@"SELECT * FROM fund_data_day WHERE jzrq >= @startTime AND jzrq <= @endTime AND code=@code ORDER BY jzrq ASC", new { code = fundCode, startTime, endTime });
            var x = list.Select(w => w.Jzrq);
            var y = list.Select(w => w.JZZZL);
            decimal startJz = 0.0M;
            if (list.FirstOrDefault() != null) startJz = list.First().Jz;
            var y2 = new List<decimal>();
            foreach (var item in list)
            {
                y2.Add(Math.Round((item.Jz - startJz) / startJz * 100, 2));
            }
            return new { title = fundCode, x, y, y2 };
        }

        public object GetAllChartData(string[] fundCodes, DateTime? startDate, DateTime? endDate)
        {
            DateTime startTime = DateTime.Now.Date.AddDays(-30);
            DateTime endTime = DateTime.Now;
            if (startDate != null) startTime = startDate.Value;
            if (endDate != null) endTime = endDate.Value.AddDays(1).AddMilliseconds(-1);

            var list = dapperHelper.GetList<FundDataDay>($"SELECT * FROM fund_data_day WHERE jzrq >= @startTime AND jzrq <= @endTime AND code IN ({string.Join(",", fundCodes)}) ORDER BY jzrq ASC", new {  startTime, endTime });
            var x = list.Select(w => w.Jzrq).Distinct().OrderBy(w => w);

            List<object> listY = new List<object>();
            foreach (var funCode in fundCodes)
            {
                var temp = list.FindAll(w => w.Code == funCode).OrderBy(w => w.Jzrq).ToList();
                if (temp?.Count > 0)
                {
                    decimal startJz = 0.0M;
                    if (list.FirstOrDefault() != null)
                    {
                        startJz = temp.First().Jz;
                        var y = new List<decimal>();
                        foreach (var item in temp)
                        {
                            y.Add(Math.Round((item.Jz - startJz) / startJz * 100, 2));
                        }
                        listY.Add(new { code = temp.First().Code, name = temp.First().Name, y });
                    }
                }
            }
            return new { x, listY };
        }

        public void GetDataTask(string fundCode = "257070")
        {
            //string fundCode = "257070";

            string url = "http://api.fund.eastmoney.com/f10/lsjz";
            var client = new RestClient();
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Host", "api.fund.eastmoney.com");
            request.AddHeader("Referer", $"http://fundf10.eastmoney.com/jjjz_{fundCode}.html");

            request.AddQueryParameter("fundCode", fundCode);
            request.AddQueryParameter("callback", "jQuery183010134522908519239_1572851114406");
            request.AddQueryParameter("_", "1572851448663");
            request.AddQueryParameter("pageIndex", "1");
            request.AddQueryParameter("pageSize", "100");
            request.AddQueryParameter("startDate", "");
            request.AddQueryParameter("endDate", "");
            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Regex regex = new Regex(@"\[\{.*\}\]");
            var str = regex.Match(content).Value;
            var list = JsonConvert.DeserializeObject<List<DayJZDto>>(str);
            if (list?.Count > 0)
            {
                var fc = dapperHelper.Get<FundCode>(@"SELECT * FROM fund_code WHERE code=@code;", new { code = fundCode });

                var listEntity = new List<FundDataDay>();
                foreach (var item in list)
                {
                    var fundDataDay = new FundDataDay
                    {
                        Code = fundCode,
                        Name = fc.Name,
                        Jzrq = item.FSRQ,
                        Jz = item.DWJZ,
                        Ljjz = item.LJJZ,
                        JZZZL = item.JZZZL,
                    };
                    var fdd = dapperHelper.Get<FundDataDay>(@"SELECT * FROM fund_data_day WHERE code=@code AND jzrq=@jzrq;", new { code = fundCode, jzrq = item.FSRQ });
                    if (fdd == null)
                    {
                        listEntity.Add(fundDataDay);
                    }
                }
                if (listEntity.Count > 0)
                {
                    dapperHelper.Insert(listEntity);
                }
            }
        }

    }
}
