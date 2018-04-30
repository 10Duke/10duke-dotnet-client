using System;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.EntApi.Authz;
using Tenduke.Client.Util;

namespace Tenduke.Client
{
    /// <summary>
    /// Basic client for working directly against the 10Duke Entitlement service.
    /// This client uses the OAuth 2.0 Authorization Code Grant flow for authorizing
    /// this client directly against the 10Duke Entitlement service.
    /// </summary>
    public class TendukeClient
    {
        #region Private fields

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// </summary>
        private AuthzApiConfig authzApiConfig;

        #endregion

        #region Properties

        /// <summary>
        /// OAuth 2.0 configuration to use for communicating with the 10Duke Entitlement service
        /// using the Authorization Code Grant flow.
        /// </summary>
        public AuthorizationCodeGrantConfig OAuthConfig { get; set; }

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// If not specified by explicitly setting this property value, default configuration is inferred from
        /// <see cref="OAuthConfig"/>.
        /// </summary>
        public AuthzApiConfig AuthzApiConfig
        {
            get
            {
                return authzApiConfig ?? AuthzApiConfig.FromOAuthConfig(OAuthConfig);
            }

            set
            {
                authzApiConfig = value;
            }
        }

        /// <summary>
        /// Authorization process result information received from the 10Duke Entitlement service.
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        /// <summary>
        /// Gets an <see cref="AuthzApi"/> object for accessing the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// Please note that the OAuth authentication / authorization process must be successfully completed before
        /// getting the <see cref="AuthzApi"/> object.
        /// </summary>
        public AuthzApi AuthzApi
        {
            get
            {
                var authzApiConfig = AuthzApiConfig;
                if (authzApiConfig == null)
                {
                    throw new InvalidOperationException("Configuration for AuthzApi missing, please specify either AuthzApiConfig or OAuthConfig");
                }

                if (Authorization == null)
                {
                    throw new InvalidOperationException("OAuth authorization must be negotiated with the server before accessing the AuthzApi");
                }

                if (Authorization.Error != null)
                {
                    throw new InvalidOperationException(
                        string.Format("OAuth authorization has not been completed successfully (error code {0}, error message \"{1}\")",
                        Authorization.Error,
                        Authorization.ErrorDescription ?? ""));
                }

                return new AuthzApi()
                {
                    AuthzApiConfig = authzApiConfig,
                    AccessToken = Authorization.AccessTokenResponse
                };
            }
        }

        /// <summary>
        /// Gets an <see cref="Util.AuthorizationSerializer"/> for reading and writing <see cref="Authorization"/>
        /// of this object by binary serialization.
        /// </summary>
        public AuthorizationSerializer AuthorizationSerializer
        {
            get
            {
                return new AuthorizationSerializer() { TendukeClient = this };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this client object currently contains a valid access token in <see cref="Authorization"/>.
        /// Access token is used for 10Duke Entitlement Service API requests.
        /// </summary>
        /// <returns><c>true</c> if authorized, <c>false</c> otherwise.</returns>
        public bool IsAuthorized()
        {
            return Authorization != null && Authorization.AccessTokenResponse != null;
        }

        /// <summary>
        /// Discards authorization information received from the server by setting <see cref="Authorization"/> to <c>null</c>.
        /// </summary>
        public void ClearAuthorization()
        {
            Authorization = null;
        }

        #endregion
    }
}
