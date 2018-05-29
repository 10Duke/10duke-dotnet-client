using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Owin;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
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

            services.AddAuthorization(options =>
            {
                options.AddPolicy(RequireAuthentication.REQUIRE_AUTHENTICATION_POLICY_NAME, policy =>
                    policy.RequireAuthenticatedUser());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Use authentication and enable working behind a reverse proxy
            app.UseAuthentication();
            app.UseRequireAuthentication();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            // Use OWIN for handling data requests
            app.Use(async (context, next) =>
            {
                var accessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                if (accessToken != null)
                {
                    var owinEnvironment = new OwinEnvironment(context);
                    owinEnvironment.Append(new KeyValuePair<string, object>(OpenIdConnectParameterNames.AccessToken, accessToken));
                }

                await next.Invoke();
            });
            app.UseOwinUserInfo();
            //app.UseOwin(pipeline =>
            //{
            //});

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
