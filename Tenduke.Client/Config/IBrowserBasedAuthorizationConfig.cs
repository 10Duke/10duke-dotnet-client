using System;
using System.Security.Cryptography;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Interface to be implemented by OAuth configuration for such OAuth flows that typically use
    /// a web browser (or a similar user agent) for implementing the flow. These flows include the
    /// Authorization Code Grant flow and the Implicit Grant flow.
    /// </summary>
    public interface IBrowserBasedAuthorizationConfig : IOAuthConfig
    {
        #region Properties

        /// <summary>
        /// The redirect Uri for redirecting back to the client from the server.
        /// </summary>
        string RedirectUri { get; }

        /// <summary>
        /// Uri of the OAuth 2.0 authorization endpoint of the 10Duke Entitlement service.
        /// </summary>
        string AuthzUri { get; }

        /// <summary>
        /// OpenID Connect ID token issuer.
        /// </summary>
        string Issuer { get; }

        /// <summary>
        /// RSA public key for verifying signatures of OpenID Connect ID Tokens received from
        /// the 10Duke Entitlement Service.
        /// </summary>
        RSA SignerKey { get; }

        #endregion
    }
}
