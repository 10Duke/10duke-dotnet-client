using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tenduke.Client.AspNetSampleOwin
{
    public class RequireAuthentication
    {
        public const string REQUIRE_AUTHENTICATION_POLICY_NAME = "RequireAuthentication";

        private readonly RequestDelegate next;

        public RequireAuthentication(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, IAuthorizationService authorizationService)
        {
            var authorized = await authorizationService.AuthorizeAsync(context.User, null, REQUIRE_AUTHENTICATION_POLICY_NAME);
            if (!authorized.Succeeded)
            {
                await context.ChallengeAsync();
                return;
            }

            await next(context);
        }
    }

    public static class RequireAuthenticationExtensions
    {
        public static IApplicationBuilder UseRequireAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequireAuthentication>();
        }
    }
}
