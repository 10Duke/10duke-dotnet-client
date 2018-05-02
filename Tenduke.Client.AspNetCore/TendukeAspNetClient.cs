using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Tenduke.Client;
using Tenduke.Client.Config;

namespace Tenduke.Client.AspNet
{
    /// <summary>
    /// Client for working with the 10Duke Identity and Entitlement Service. Uses OAuth 2.0 Authorization Code
    /// Grant flow for authorization.
    /// </summary>
    public class TendukeAspNetClient : BaseClient<IAuthorizationCodeGrantConfig>
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="HttpRequest"/> representing the HTTP request from the user agent.
        /// </summary>
        public HttpRequest Request { get; set; }

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// If not specified by explicitly setting this property value, default configuration is inferred from
        /// <see cref="OAuthConfig"/>.
        /// </summary>
        public new IAuthzApiConfig AuthzApiConfig
        {
            get
            {
                return base.AuthzApiConfig ?? Config.AuthzApiConfig.FromOAuthConfig(OAuthConfig);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/> representing the HTTP request from the user agent.</param>
        /// <returns></returns>
        public static TendukeAspNetClient Build(HttpRequest request)
        {
            var retValue = new TendukeAspNetClient()
            {
                Request = request
            };

            return retValue;
        }

        /// <summary>
        /// Loads JSON configuration from <c>appsettings.json</c> configuration file, and sets the <see cref="OAuthConfig"/>
        /// property of this instance.
        /// </summary>
        protected void LoadJsonConfiguration()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();

            var oauthConfig = ConfigurationReader.AuthorizationCodeGrantConfigFromConfiguration(config);
            OAuthConfig = oauthConfig;
        }

        #endregion
    }
}
