using Newtonsoft.Json;
using RestSharp;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.Services
{
    public class FundCodeService
    {
        private readonly string fundcode_url = "http://fund.eastmoney.com/js/fundcode_search.js";

        private readonly DapperHelper dapperHelper;
        public FundCodeService(DapperHelper dapperHelper)
        {
            this.dapperHelper = dapperHelper;
        }

        public void Insert()
        {
            var client = new RestClient();
            var request = new RestRequest(fundcode_url);
            var response = client.Get(request);
            var content = response.Content;
            content = content.Replace("var r = ", "").Replace(";", "").Replace("\"", "");
            content = content.Substring(2, content.Length - 3);
            var arr1 = content.Split("],[");

            if (dapperHelper.Get<FundCode>(@"SELECT * FROM fund_code") == null)
            {
                foreach (var item in arr1)
                {
                    var arr2 = item.Split(',');
                    var fundCode = new FundCode
                    {
                        Code = arr2[0],
                        PinYin = arr2[1],
                        Name = arr2[2],
                        Type = arr2[3],
                        Extend1 = arr2[4]
                    };
                    dapperHelper.Insert(fundCode);
                }
                dapperHelper.CloseDbConnection();
            }
        }

        public string AddGatherCode(string code)
        {
            string msg = "操作成功";
            var fundCode = dapperHelper.Get<FundCode>(@"SELECT * FROM fund_code WHERE code=@code", new { code });
            if (fundCode == null)
            {
                msg = "未找到基金代码";
            }
            else
            {
                var entity = dapperHelper.Get<FundGatherList>(@"SELECT * FROM fund_gather_list WHERE code=@code", new { code });
                if (entity != null)
                {
                    msg = "该代码已加入收集列表";
                }
                else
                {
                    dapperHelper.Insert(new FundGatherList { Code = code, Name = fundCode.Name });
                }
            }
            return msg;
        }

        public List<FundGatherList> GetGatherList()
        {
            return dapperHelper.GetList<FundGatherList>(@"SELECT * FROM fund_gather_list");
        }
    }
}
