using System;
using System.Security.Cryptography;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Interface for configuration of the 10Duke Authz API
    /// </summary>
    public interface IAuthzApiConfig
    {
        /// <summary>
        /// Uri of the <c>/authz/</c> endpoint.
        /// </summary>
        string EndpointUri { get; }

        /// <summary>
        /// RSA public key for verifying signatures of signed JWT authorization decision
        /// tokens returned by the <c>/authz/</c> API.
        /// </summary>
        RSA SignerKey { get; }

        /// <summary>
        /// Indicates if insecure certificates are accepted when communicating with the server.
        /// </summary>
        bool AllowInsecureCerts { get; }
    }
}
