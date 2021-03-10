using System;
using System.Security.Cryptography;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Wraps another <see cref="IBrowserBasedAuthorizationConfig"/> and allows selective overriding.
    /// </summary>
    public abstract class BrowserBasedAuthorizationConfigWrapper : IBrowserBasedAuthorizationConfig
    {
        /// <summary>
        /// The wrapped config object.
        /// </summary>
        protected IBrowserBasedAuthorizationConfig Wrapped { get; set; }

        /// <summary>
        /// The redirect Uri for redirecting back to the client from the server.
        /// Please note that there are authentication flows that use a dynamic redirect
        /// uri. When using this kind of an authentication flow, the configured
        /// redirect uri is ignored.
        /// </summary>
        public virtual string RedirectUri
        {
            get
            {
                return AssertWrapped().RedirectUri;
            }
        }

        /// <summary>
        /// Uri of the OAuth 2.0 authorization endpoint of the 10Duke Entitlement service.
        /// </summary>
        public virtual string AuthzUri
        {
            get
            {
                return AssertWrapped().AuthzUri;
            }
        }

        /// <summary>
        /// OpenID Connect ID token issuer.
        /// </summary>
        public virtual string Issuer
        {
            get
            {
                return AssertWrapped().Issuer;
            }
        }

        /// <summary>
        /// RSA public key for verifying signatures of OpenID Connect ID Tokens received from
        /// the 10Duke Entitlement Service.
        /// </summary>
        public virtual RSA SignerKey
        {
            get
            {
                return AssertWrapped().SignerKey;
            }
        }

        /// <summary>
        /// The OAuth 2.0 client id.
        /// </summary>
        public virtual string ClientID
        {
            get
            {
                return AssertWrapped().ClientID;
            }
        }

        /// <summary>
        /// The OAuth 2.0 scope. If using OpenID Connect, <c>openid</c> scope value must be included.
        /// </summary>
        /// <example>openid profile email</example>
        public virtual string Scope
        {
            get
            {
                return AssertWrapped().Scope;
            }
        }

        /// <summary>
        /// Uri of the OpenID Connect userinfo endpoint of the 10Duke Entitlement service.
        /// </summary>
        public virtual string UserInfoUri
        {
            get
            {
                return AssertWrapped().UserInfoUri;
            }
        }

        /// <summary>
        /// Indicates if user should be allowed to select OAuth session lifetime preference.
        /// </summary>
        public virtual bool ShowRememberMe
        {
            get
            {
                return AssertWrapped().ShowRememberMe;
            }
        }

        /// <summary>
        /// Indicates if insecure certificates are accepted when communicating with the server.
        /// </summary>
        public virtual bool AllowInsecureCerts
        {
            get
            {
                return AssertWrapped().AllowInsecureCerts;
            }
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="BrowserBasedAuthorizationConfigWrapper"/> class.
        /// </summary>
        /// <param name="wrapped">The wrapped config object.</param>
        protected BrowserBasedAuthorizationConfigWrapper(IBrowserBasedAuthorizationConfig wrapped)
        {
            Wrapped = wrapped;
        }

        /// <summary>
        /// Returns value of <see cref="Wrapped"/>, asserting that it is not <c>null</c>.
        /// </summary>
        /// <returns>The wrapped <see cref="IBrowserBasedAuthorizationConfig"/> object.</returns>
        protected IBrowserBasedAuthorizationConfig AssertWrapped()
        {
            var retValue = Wrapped;
            if (retValue == null)
            {
                throw new InvalidOperationException("Wrapped configuration object must not be null");
            }

            return retValue;
        }
    }
}
