using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FundApp.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            return true;

            var httpContext = context.GetHttpContext();

            //var header = httpContext.Request.Headers["Authorization"];

            //if (string.IsNullOrWhiteSpace(header))
            //{
            //    SetChallengeResponse(httpContext);
            //    return false;
            //}

            //var authValues = System.Net.Http.Headers.AuthenticationHeaderValue.Parse(header);

            //if (!"Basic".Equals(authValues.Scheme, StringComparison.InvariantCultureIgnoreCase))
            //{
            //    SetChallengeResponse(httpContext);
            //    return false;
            //}

            //var parameter = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));
            //var parts = parameter.Split(':');

            //if (parts.Length < 2)
            //{
            //    SetChallengeResponse(httpContext);
            //    return false;
            //}

            //var username = parts[0];
            //var password = parts[1];

            //if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            //{
            //    SetChallengeResponse(httpContext);
            //    return false;
            //}

            //if (username == "user" && password == "paw")
            //{
            //    return true;
            //}

            //SetChallengeResponse(httpContext);
            //return false;
        }
    }
}
