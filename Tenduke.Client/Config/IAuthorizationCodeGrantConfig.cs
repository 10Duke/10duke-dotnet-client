using System;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Interface to be implemented by configuration for OAuth Authorization Code Grant flow.
    /// </summary>
    public interface IAuthorizationCodeGrantConfig : IBrowserBasedAuthorizationConfig
    {
        #region Properties

        /// <summary>
        /// OAuth 2.0 client secret.
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// Uri of the OAuth 2.0 token endpoint of the 10Duke Entitlement service.
        /// </summary>
        string TokenUri { get; }

        #endregion
    }
}
