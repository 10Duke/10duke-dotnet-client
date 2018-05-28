﻿using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tenduke.Client.AspNetSampleOwin
{
    public class OwinAuthenticationMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> nextFunc;

        public OwinAuthenticationMiddleware(Func<IDictionary<string, object>, Task> nextFunc)
        {
            this.nextFunc = nextFunc;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return nextFunc(environment);
        }
    }

    public static class OwinAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseOwinAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseOwin(setup => setup(next =>
            {
                return new OwinAuthenticationMiddleware(next).Invoke;
            }));
        }
    }
}
