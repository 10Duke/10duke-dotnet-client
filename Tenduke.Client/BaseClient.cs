using System;
using System.Net.Http;
using Tenduke.Client.Config;
using Tenduke.Client.EntApi.Authz;
using Tenduke.Client.UserInfo;

namespace Tenduke.Client
{
    /// <summary>
    /// Base class for clients for working against the 10Duke Entitlement service.
    /// </summary>
    public class BaseClient<A> where A : IOAuthConfig
    {
        #region Private fields

        /// <summary>
        /// The <see cref="HttpClient"/> for 10Duke API calls.
        /// </summary>
        protected static readonly HttpClient HttpClient = new();

        #endregion

        #region Properties

        /// <summary>
        /// OAuth 2.0 configuration to use for communicating with the 10Duke Entitlement service.
        /// </summary>
        public A OAuthConfig { get; set; }

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// </summary>
        public virtual IAuthzApiConfig AuthzApiConfig { get; set; }

        /// <summary>
        /// OAuth 2.0 access token for accessing APIs that require authorization.
        /// </summary>
        public virtual string AccessToken { get; set; }

        /// <summary>
        /// Gets an <see cref="AuthzApi"/> object for accessing the <c>/authz/</c> API of the 10Duke Identity and Entitlement service.
        /// Please note that the OAuth authentication / authorization process must be successfully completed before
        /// getting the <see cref="AuthzApi"/> object, and the <see cref="AccessToken"/> must be available.
        /// </summary>
        public AuthzApi AuthzApi
        {
            get
            {
                var authzApiConfig = AuthzApiConfig ?? throw new InvalidOperationException("Configuration for AuthzApi missing, please specify either AuthzApiConfig or OAuthConfig");
                var accessToken = AccessToken;
                return accessToken == null
                    ? throw new InvalidOperationException("AccessToken must be specified for using the AuthzApi")
                    : new AuthzApi()
                {
                    HttpClient = HttpClient,
                    AuthzApiConfig = authzApiConfig,
                    AccessToken = accessToken
                };
            }
        }

        /// <summary>
        /// Gets an <see cref="UserInfoApi"/> object for accessing the <c>/userinfo</c> API of the 10Duke Identity and Entitlement service.
        /// Please note that the OAuth authentication / authorization process must be successfully completed before
        /// getting the <see cref="UserInfoApi"/> object, and the <see cref="AccessToken"/> must be available.
        /// </summary>
        public UserInfoApi UserInfoApi
        {
            get
            {
                var oauthConfig = OAuthConfig ?? throw new InvalidOperationException("OAuthConfig must be specified");
                var accessToken = AccessToken;
                return accessToken == null
                    ? throw new InvalidOperationException("AccessToken must be specified for using the AuthzApi")
                    : new UserInfoApi()
                {
                    HttpClient = HttpClient,
                    OAuthConfig = oauthConfig,
                    AccessToken = accessToken
                };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this client object currently contains a valid access token in <see cref="AccessToken"/>.
        /// Access token is used for 10Duke Entitlement Service API requests.
        /// </summary>
        /// <returns><c>true</c> if authorized, <c>false</c> otherwise.</returns>
        public bool IsAuthorized()
        {
            return AccessToken != null;
        }

        /// <summary>
        /// Discards authorization information received from the server by setting <see cref="AccessToken"/> to <c>null</c>.
        /// </summary>
        public void ClearAuthorization()
        {
            AccessToken = null;
        }

        #endregion
    }
}
