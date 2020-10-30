using System;
using System.Security.Cryptography;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Configuration for accessing the <c>/authz/</c> API.
    /// </summary>
    [Serializable]
    public class AuthzApiConfig : IAuthzApiConfig
    {
        /// <summary>
        /// Uri path for building default Uri for accessing the <c>/authz/</c> API.
        /// </summary>
        public static readonly string DEFAULT_AUTHZ_PATH = "/authz/";

        /// <summary>
        /// Uri of the <c>/authz/</c> endpoint.
        /// </summary>
        public string EndpointUri { get; set; }

        /// <summary>
        /// RSA public key for verifying signatures of signed JWT authorization decision
        /// tokens returned by the <c>/authz/</c> API.
        /// </summary>
        public RSA SignerKey { get; set; }

        /// <summary>
        /// Indicates if insecure certificates are accepted when communicating with the server.
        /// </summary>
        public bool AllowInsecureCerts { get; set; }

        /// <summary>
        /// Initializes an <see cref="IAuthzApiConfig"/> instance based on the given <paramref name="browserBasedAuthorizationConfig"/>
        /// object representing OAuth 2.0 configuration used for connecting to the 10Duke Entitlement service.
        /// This method assumes that the <c>/authz/</c> endpoint is on the same host as the OAuth 2.0 authorization
        /// endpoint, that the <c>/authz/</c> endpoint is in the root context.
        /// </summary>
        /// <param name="browserBasedAuthorizationConfig"><see cref="IBrowserBasedAuthorizationConfig"/> representing OAuth 2.0 configuration
        /// for connecting to the 10Duke Entitlement service. If <c>null</c>, this method returns <c>null</c>.</param>
        /// <returns>The <see cref="IAuthzApiConfig"/> object, or <c>null</c> if <paramref name="browserBasedAuthorizationConfig"/> is <c>null</c>.</returns>
        public static IAuthzApiConfig FromOAuthConfig(IBrowserBasedAuthorizationConfig browserBasedAuthorizationConfig)
        {
            return browserBasedAuthorizationConfig == null
                ? null
                : new AuthzApiConfig()
                {
                    EndpointUri = BuildDefaultAuthzUri(browserBasedAuthorizationConfig.AuthzUri).ToString(),
                    SignerKey = browserBasedAuthorizationConfig.SignerKey,
                    AllowInsecureCerts = browserBasedAuthorizationConfig.AllowInsecureCerts
                };
        }

        /// <summary>
        /// Builds default Uri for accessing the <c>/authz/</c> endpoint based on the OAuth
        /// authorization Uri.
        /// </summary>
        /// <param name="oauthEndpointUri">Uri of the OAuth 2.0 authorization endpoint of the 10Duke Entitlement service.</param>
        /// <returns>The default Uri for accessing the <c>/authz/</c> endpoint.</returns>
        private static Uri BuildDefaultAuthzUri(string oauthEndpointUri)
        {
            return new Uri(new Uri(oauthEndpointUri), AuthzApiConfig.DEFAULT_AUTHZ_PATH);
        }
    }
}
