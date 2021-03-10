using System;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.Desktop.Config;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.EntApi.Authz;

namespace Tenduke.Client.WinBase
{
    /// <summary>
    /// Base class for desktop clients working against the 10Duke Entitlement service and using
    /// the CefSharp embedded browser for user interaction.
    /// </summary>
    public class BaseWinClient<C, A> : BaseClient<C, A> where C : BaseWinClient<C, A> where A : IAuthorizationCodeGrantConfig
    {
        #region Private fields

        /// <summary>
        /// Identifier of this system, used when communicating with the authorization
        /// endpoint of the 10Duke Entitlement service.
        /// </summary>
        private string computerId;

        #endregion

        #region Properties

        /// <summary>
        /// Authorization process result information received from the 10Duke Entitlement service.
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        /// <summary>
        /// Configuration specifying how this system is identified when communicating with the authorization
        /// endpoint of the 10Duke Entitlement service. If <c>null</c>, default configuration for computer
        /// identity computing is used.
        /// </summary>
        public ComputerIdentityConfig ComputerIdentityConfig { get; set; }

        /// <summary>
        /// Gets the identifier of this system, used when communicating with the authorization
        /// endpoint of the 10Duke Entitlement service.
        /// </summary>
        public string ComputerId
        {
            get
            {
                var retValue = computerId;
                if (retValue == null)
                {
                    ComputerIdentity.ComputerIdentifier[] idComponents =
                        ComputerIdentityConfig == null || (ComputerIdentityConfig.ComputeBy == null && ComputerIdentityConfig.AdditionalIdentifier == null)
                        ? new[] { ComputerIdentity.ComputerIdentifier.BaseboardSerialNumber } // Uses BaseboardSerialNumber as default
                        : ComputerIdentityConfig.ComputeBy;
                    if (ComputerIdentityConfig == null)
                    {
                        retValue = ComputerIdentity.BuildComputerId(null, null, idComponents);
                    }
                    else if (ComputerIdentityConfig.ComputerId == null)
                    {
                        retValue = ComputerIdentity.BuildComputerId(ComputerIdentityConfig.AdditionalIdentifier, ComputerIdentityConfig.Salt, idComponents);
                    }
                    else
                    {
                        retValue = ComputerIdentityConfig.ComputerId;
                    }

                    computerId = retValue;
                }

                return retValue;
            }
        }

        /// <summary>
        /// OAuth 2.0 access token for accessing APIs that require authorization.
        /// </summary>
        public override string AccessToken
        {
            get
            {
                return Authorization == null || Authorization.AccessTokenResponse == null ? null : Authorization.AccessTokenResponse.AccessToken;
            }

            set
            {
                throw new InvalidOperationException("AccessToken can not be set directly, set Authorization instead");
            }
        }

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// </summary>
        public override IAuthzApiConfig AuthzApiConfig
        {
            get
            {
                return base.AuthzApiConfig ?? Client.Config.AuthzApiConfig.FromOAuthConfig(OAuthConfig);
            }

            set
            {
                base.AuthzApiConfig = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="AuthzApi"/> object for accessing the <c>/authz/</c> API of the 10Duke Identity and Entitlement service.
        /// Please note that the OAuth authentication / authorization process must be successfully completed before
        /// getting the <see cref="AuthzApi"/> object, and the <see cref="AccessToken"/> must be available.
        /// </summary>
        public new AuthzApi AuthzApi
        {
            get
            {
                var retValue = base.AuthzApi;
                retValue.ComputerId = ComputerId;
                return retValue;
            }
        }

        /// <summary>
        /// Gets an <see cref="EntClientAuthorizationSerializer"/> for reading and writing <see cref="Authorization"/>
        /// of this object by binary serialization.
        /// </summary>
        public EntClientAuthorizationSerializer<C, A> AuthorizationSerializer
        {
            get
            {
                return new EntClientAuthorizationSerializer<C, A>() { EntClient = (C) this };
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Discards authorization information received from the server by setting <see cref="Authorization"/> to <c>null</c>.
        /// </summary>
        public new void ClearAuthorization()
        {
            Authorization = null;
        }

        #endregion
    }
}
