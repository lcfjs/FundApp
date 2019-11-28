using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FundDataMinService
    {
        private readonly DapperHelper dapperHelper = null;

        public FundDataMinService(DapperHelper dapperHelper)
        {
            this.dapperHelper = dapperHelper;
        }

        public List<FundDataMin> GetPageList(string fundCode, DateTime? startDate, DateTime? endDate, out int total, int pageIndex = 0, int pageSize = 20)
        {
            DateTime startTime = DateTime.Now.Date.AddDays(-30);
            DateTime endTime = DateTime.Now;
            if (startDate != null) startTime = startDate.Value;
            if (endDate != null) endTime = endDate.Value.AddDays(1).AddMilliseconds(-1);

            var sql = @"SELECT COUNT(1) FROM fund_data_min WHERE jzsj >= @startTime AND jzsj <= @endTime AND code=@code;
                        SELECT * FROM fund_data_min WHERE jzsj >= @startTime AND jzsj <= @endTime AND code=@code ORDER BY jzsj ASC LIMIT @pageIndex,@pageSize";
            var totalCount = 0;
            List<FundDataMin> list = null;
            dapperHelper.QueryMultiple(sql, (reader) =>
            {
                totalCount = reader.ReadFirst<int>();
                list = reader.Read<FundDataMin>().ToList();
            }, new { code = fundCode, startTime, endTime, pageIndex = pageIndex * pageSize, pageSize });
            total = totalCount;
            return list;
        }

        public object GetChartData(string fundCode)
        {
            DateTime startTime = DateTime.Now.Date;
            DateTime endTime = DateTime.Now;

            var list = dapperHelper.GetList<FundDataMin>(@"SELECT * FROM fund_data_min WHERE jzsj >= @startTime AND jzsj <= @endTime AND code=@code ORDER BY jzsj ASC", new { code = fundCode, startTime, endTime });
            var x = list.Select(w => w.Jzsj);
            var y1 = list.Select(w => w.Gsz);
            var y2 = list.Select(w => w.Gssy);
            return new { title = fundCode, x, y1, y2 };
        }

        public void GetDataTask(string fundCode = "257070")
        {
            //string fundCode = "257070";

            string url = $"http://fundgz.1234567.com.cn/js/{fundCode}.js";
            var client = new RestClient();
            var request = new RestRequest(url, Method.GET);
            request.AddHeader("Host", "fundgz.1234567.com.cn");
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            IRestResponse response = client.Execute(request);
            var content = response.Content;

            Regex regex = new Regex(@"\{.*\}");
            var str = regex.Match(content).Value;
            var jobj = JsonConvert.DeserializeObject<JObject>(str);

            if (jobj != null)
            {
                //jsonpgz({"fundcode":"257070","name":"国联安优选行业混合","jzrq":"2019-11-04","dwjz":"1.8290","gsz":"1.8297","gszzl":"0.04","gztime":"2019-11-05 09:45"});
                var fundDataDay = new FundDataMin
                {
                    Code = fundCode,
                    Name = jobj["name"].ToString(),
                    Jzsj = Convert.ToDateTime(jobj["gztime"].ToString()),
                    Gsz = Convert.ToDecimal(jobj["gsz"].ToString()),
                    Zrjz = Convert.ToDecimal(jobj["dwjz"].ToString()),
                    Gssy = Convert.ToDecimal(jobj["gszzl"].ToString()),
                };
                var fdm = dapperHelper.Get<FundDataMin>(@"SELECT * FROM fund_data_min WHERE code=@code AND jzsj=@jzsj", new { code = fundCode, jzsj = fundDataDay.Jzsj });
                if (fdm == null)
                {
                    dapperHelper.Insert(fundDataDay);
                }
            }
        }
    }
}
