using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenduke.Client.AspNetCore;

namespace Tenduke.Client.AspNetSampleOwin
{
    /// <summary>
    /// OWIN middleware that handles requests to <c>/api/SampleData/UserInfo</c>,
    /// and calls 10Duke Identity and Entitlement Service for requesting user info.
    /// </summary>
    public class OwinUserInfoMiddleware
    {
        public const string REQUEST_PATH_USER_INFO = "/api/SampleData/UserInfo";

        private readonly Func<IDictionary<string, object>, Task> nextFunc;

        public OwinUserInfoMiddleware(Func<IDictionary<string, object>, Task> nextFunc)
        {
            this.nextFunc = nextFunc;
        }

        public async Task Invoke(IDictionary<string, object> environment)
        {
            var requestPath = (string) environment["owin.RequestPath"];
            if (REQUEST_PATH_USER_INFO == requestPath)
            {
                var httpContext = (HttpContext)environment["Microsoft.AspNetCore.Http.HttpContext"];
                var accessToken = await httpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                var tendukeClient = TendukeClient.Build(accessToken);

                var userInfo = await tendukeClient.UserInfoApi.GetUserInfoAsync();
                var jsonResponse = JsonConvert.SerializeObject(userInfo);
                var responseBytes = Encoding.UTF8.GetBytes(jsonResponse);

                var responseStream = (Stream)environment["owin.ResponseBody"];
                var responseHeaders = (IDictionary<string, string[]>)environment["owin.ResponseHeaders"];
                responseHeaders["Content-Length"] = new string[] { responseBytes.Length.ToString(CultureInfo.InvariantCulture) };
                responseHeaders["Content-Type"] = new string[] { "application/json" };
                await responseStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            }
            else
            {
                await nextFunc(environment);
            }
        }
    }

    public static class OwinAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseOwinUserInfo(this IApplicationBuilder builder)
        {
            return builder.UseOwin(setup => setup(next =>
            {
                return new OwinUserInfoMiddleware(next).Invoke;
            }));
        }
    }
}
