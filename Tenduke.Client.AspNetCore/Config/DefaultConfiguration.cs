using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tenduke.Client.Config;
using Tenduke.Client.Util;

namespace Tenduke.Client.AspNetCore.Config
{
    /// <summary>
    /// Utility for working with configuration loaded from <c>appsettings.json</c> JSON configuration file.
    /// </summary>
    public sealed class DefaultConfiguration
    {
        #region Public constants

        /// <summary>
        /// Name of configuration key for the 10Duke Identity and Entitlement service configuration.
        /// </summary>
        public static readonly string CONFIGURATION_KEY_TENDUKE_CONFIG = "Tenduke";

        /// <summary>
        /// Name of configuration key for OAuth / OpenID Connect authorization endpoint URI (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_AUTHZ_URI = "AuthzUri";

        /// <summary>
        /// Name of configuration key for OAuth / OpenID Connect access token endpoint URI (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_TOKEN_URI = "TokenUri";

        /// <summary>
        /// Name of configuration key for OpenID Connect user info endpoint URI (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_USER_INFO_URI = "UserInfoUri";

        /// <summary>
        /// Name of configuration key for OpenID Connect ID token issuer, used for verifying ID tokens (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_ISSUER = "Issuer";

        /// <summary>
        /// Name of configuration key for OAuth client id (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_CLIENT_ID = "ClientID";

        /// <summary>
        /// Name of configuration key for OAuth client secret (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_CLIENT_SECRET = "ClientSecret";

        /// <summary>
        /// Name of configuration key for OAuth callback redirect URI (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_REDIRECT_URI = "RedirectUri";

        /// <summary>
        /// Name of configuration key for OAuth scope (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_SCOPE = "Scope";

        /// <summary>
        /// Name of configuration key for RSA public key to use for verifying signatures (subkey of <see cref="CONFIGURATION_KEY_TENDUKE_CONFIG"/>).
        /// </summary>
        public static readonly string CONFIGURATION_KEY_SIGNER_KEY = "SignerKey";

        #endregion

        #region Methods

        /// <summary>
        /// Loads OAuth / OpenID Connect configuration for connecting to the 10Duke Identity and Entitlement Service.
        /// </summary>
        /// <returns>Returns an <see cref="IAuthorizationCodeGrantConfig"/> object containing values parsed from the configuration.</returns>
        public static IAuthorizationCodeGrantConfig LoadOAuthConfiguration()
        {
            var config = BuildConfiguration();
            var tendukeConfig = config.GetSection(CONFIGURATION_KEY_TENDUKE_CONFIG);
            return ConfigurationReader.AuthorizationCodeGrantConfigFromConfiguration(tendukeConfig);
        }

        /// <summary>
        /// Loads OAuth / OpenID Connect configuration for connecting to the 10Duke Identity and Entitlement Service.
        /// </summary>
        /// <param name="options">An <see cref="OpenIdConnectOptions"/> object for setting values parsed from the configuration.</param>
        public static void LoadOpenIdConnectOptions(OpenIdConnectOptions options)
        {
            var config = BuildConfiguration();
            var tendukeConfig = config.GetSection(CONFIGURATION_KEY_TENDUKE_CONFIG);

            options.ClientId = tendukeConfig[CONFIGURATION_KEY_CLIENT_ID];
            options.ClientSecret = tendukeConfig[CONFIGURATION_KEY_CLIENT_SECRET];
            options.Configuration = new OpenIdConnectConfiguration()
            {
                AuthorizationEndpoint = tendukeConfig[CONFIGURATION_KEY_AUTHZ_URI],
                Issuer = tendukeConfig[CONFIGURATION_KEY_ISSUER],
                TokenEndpoint = tendukeConfig[CONFIGURATION_KEY_TOKEN_URI],
                UserInfoEndpoint = tendukeConfig[CONFIGURATION_KEY_USER_INFO_URI]
            };
            options.Configuration.SigningKeys.Add(new RsaSecurityKey(CryptoUtil.ReadRsaPublicKey(tendukeConfig[CONFIGURATION_KEY_SIGNER_KEY])));
            options.RequireHttpsMetadata = false;
            options.ResponseMode = OpenIdConnectResponseMode.Query;
            options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
            options.SaveTokens = true;

            if (tendukeConfig[CONFIGURATION_KEY_SCOPE] != null)
            {
                foreach (var scope in tendukeConfig[CONFIGURATION_KEY_SCOPE].Split(' '))
                {
                    options.Scope.Add(scope);
                }
            }
        }

        /// <summary>
        /// Builds <see cref="IConfigurationRoot"/> for accessing configuration in the <c>appsettings.json</c> configuration file.
        /// </summary>
        /// <returns>Returns an <see cref="IConfigurationRoot"/> object for accessing the configuration.</returns>
        public static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json").Build();
        }

        #endregion
    }
}
