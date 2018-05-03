using System;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Interface for OAuth configuration that is common for all supported OAuth flows.
    /// </summary>
    public interface IOAuthConfig
    {
        #region Properties

        /// <summary>
        /// The OAuth 2.0 client id.
        /// </summary>
        string ClientID { get; }

        /// <summary>
        /// The OAuth 2.0 scope. If using OpenID Connect, <c>openid</c> scope value must be included.
        /// </summary>
        /// <example>openid profile email</example>
        string Scope { get; }

        /// <summary>
        /// Uri of the OpenID Connect userinfo endpoint of the 10Duke Entitlement service.
        /// </summary>
        string UserInfoUri { get; }

        #endregion
    }
}
