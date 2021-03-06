using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tenduke.Client.AspNetCore.Config;

namespace Tenduke.Client.AspNetSampleOwin
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add authentication against 10Duke Identity Service using OpenID Connect
            var tendukeConfig = DefaultConfiguration.LoadOAuthConfiguration();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(configureOptions =>
            {
                DefaultConfiguration.LoadOpenIdConnectOptions(configureOptions);
            });

            // Add authorization policy for requiring authenticated user
            services.AddAuthorization(options =>
            {
                options.AddPolicy(RequireAuthentication.REQUIRE_AUTHENTICATION_POLICY_NAME, policy =>
                    policy.RequireAuthenticatedUser());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable working behind a reverse proxy
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Use authentication
            app.UseAuthentication();
            app.UseRequireAuthentication();

            // Use an OWIN middleware for handling user info requests
            app.UseOwinUserInfo();

            // Serve static files for the client-size Angular application
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.Run(async (context) =>
            {
                context.Response.ContentType = "text/html";
                await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "Index.html"));
            });

            if (env.IsDevelopment())
            {
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });
            }
        }
    }
}
