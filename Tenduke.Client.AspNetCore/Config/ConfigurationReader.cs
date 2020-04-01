using Microsoft.Extensions.Configuration;
using System;
using System.Security.Cryptography;
using Tenduke.Client.Config;
using Tenduke.Client.Util;

namespace Tenduke.Client.AspNetCore.Config
{
    /// <summary>
    /// Reading and accessing configurations required by the 10Duke client, based on an <see cref="IConfiguration" /> object.
    /// </summary>
    public class ConfigurationReader
    {
        #region Configuration interface implementations

        protected class ConfigBase
        {
            protected IConfiguration Config { get; }

            protected ConfigBase(IConfiguration config)
            {
                Config = config;
            }
        }

        protected class OAuthConfig : ConfigBase, IOAuthConfig
        {
            internal OAuthConfig(IConfiguration config)
                : base(config)
            {
            }

            public string ClientID => Config["ClientId"];

            public string Scope => Config["Scope"];

            public string UserInfoUri => Config["UserInfoUri"];

            public bool ShowRememberMe => string.IsNullOrEmpty(Config["ShowRememberMe"]) ? false : bool.Parse(Config["ShowRememberMe"]);
        }

        protected class BrowserBasedAuthorizationConfig : OAuthConfig, IBrowserBasedAuthorizationConfig
        {
            internal BrowserBasedAuthorizationConfig(IConfiguration config)
                : base(config)
            {
            }

            public string RedirectUri => Config["RedirectUri"];

            public string AuthzUri => Config["AuthzUri"];

            public string Issuer => Config["Issuer"];

            public RSA SignerKey => CryptoUtil.ReadRsaPublicKey(Config["SignerKey"]);
        }

        protected class AuthorizationCodeGrantConfig : BrowserBasedAuthorizationConfig, IAuthorizationCodeGrantConfig
        {
            internal AuthorizationCodeGrantConfig(IConfiguration config)
                : base(config)
            {
            }

            public string ClientSecret => Config["ClientSecret"];

            public string TokenUri => Config["TokenUri"];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets an object for accessing OAuth Authorization Code Grant configuration.
        /// </summary>
        /// <param name="config"><see cref="IConfiguration"/> object for accessing configuration settings.</param>
        /// <returns>An <see cref="IAuthorizationCodeGrantConfig"/> object for accessing the OAuth Authorization Code Grant configuration.</returns>
        public static IAuthorizationCodeGrantConfig AuthorizationCodeGrantConfigFromConfiguration(IConfiguration config) => new AuthorizationCodeGrantConfig(config);

        #endregion
    }
}
