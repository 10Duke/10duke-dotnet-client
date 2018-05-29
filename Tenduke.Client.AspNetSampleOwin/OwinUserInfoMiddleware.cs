using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tenduke.Client.AspNetSampleOwin
{
    public class OwinUserInfoMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> nextFunc;
        private readonly IAuthenticationService authenticationService;

        public OwinUserInfoMiddleware(Func<IDictionary<string, object>, Task> nextFunc, IAuthenticationService authenticationService)
        {
            this.nextFunc = nextFunc;
            this.authenticationService = authenticationService;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return nextFunc(environment);
        }
    }

    public static class OwinAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseOwinUserInfo(this IApplicationBuilder builder)
        {
            return builder.UseOwin(setup => setup(next =>
            {
                return new OwinUserInfoMiddleware(next, (IAuthenticationService) builder.ApplicationServices.GetService(typeof(IAuthenticationService))).Invoke;
            }));
        }
    }
}
