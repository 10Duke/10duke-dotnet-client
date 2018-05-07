﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Threading.Tasks;
using Tenduke.Client.AspNetCore.Config;
using Tenduke.Client.Config;

namespace Tenduke.Client.AspNetCore
{
    /// <summary>
    /// Client for working with the 10Duke Identity and Entitlement Service. Uses OAuth 2.0 Authorization Code
    /// Grant flow for authorization.
    /// </summary>
    public class TendukeClient : BaseClient<TendukeClient, IAuthorizationCodeGrantConfig>
    {
        #region Properties

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// If not specified by explicitly setting this property value, default configuration is inferred from
        /// <see cref="OAuthConfig"/>.
        /// </summary>
        public new IAuthzApiConfig AuthzApiConfig
        {
            get
            {
                return base.AuthzApiConfig ?? Client.Config.AuthzApiConfig.FromOAuthConfig(OAuthConfig);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a <see cref="TendukeClient"/> object for working with the 10Duke Identity and Entitlement
        /// service is the given <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> for the executing action.</param>
        /// <returns>The <see cref="TendukeClient"/> instance.</returns>
        public static async Task<TendukeClient> Build(HttpContext context)
        {
            var tendukeOAuthConfig = DefaultConfiguration.LoadOAuthConfiguration();

            var retValue = new TendukeClient()
            {
                OAuthConfig = tendukeOAuthConfig
            };
            retValue.AccessToken = await context.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            return retValue;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Loads JSON configuration from <c>appsettings.json</c> configuration file, and sets the <see cref="OAuthConfig"/>
        /// property of this instance.
        /// </summary>
        protected void LoadOAuthConfiguration()
        {
            OAuthConfig = DefaultConfiguration.LoadOAuthConfiguration();
        }

        #endregion
    }
}
