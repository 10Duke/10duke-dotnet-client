using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Base class for OAuth 2.0 configuration objects used with OAuth 2.0 flows that may
    /// be implemented using a browser-based authorization flow.
    /// </summary>
    [Serializable]
    public abstract class BrowserBasedAuthorizationConfig : OAuthConfig, IBrowserBasedAuthorizationConfig
    {
        #region Properties

        /// <summary>
        /// The redirect Uri for redirecting back to the client from the server.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Uri of the OAuth 2.0 authorization endpoint of the 10Duke Entitlement service.
        /// </summary>
        public string AuthzUri { get; set; }

        /// <summary>
        /// RSA public key for verifying signatures of OpenID Connect ID Tokens received from
        /// the 10Duke Entitlement Service.
        /// </summary>
        public RSA SignerKey { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserBasedAuthorizationConfig"/> class.
        /// </summary>
        public BrowserBasedAuthorizationConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserBasedAuthorizationConfig"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected BrowserBasedAuthorizationConfig(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            RedirectUri = info.GetString("RedirectUri");
            AuthzUri = info.GetString("AuthzUri");
            RSAParameters? rsaParameters = info.GetValue("SignerKey", typeof(RSAParameters?)) as RSAParameters?;
            if (rsaParameters != null)
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(rsaParameters.Value);
                SignerKey = rsa;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets data for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("RedirectUri", RedirectUri);
            info.AddValue("AuthzUri", AuthzUri);
            RSAParameters? rsaParameters = SignerKey == null ? (RSAParameters?)null : SignerKey.ExportParameters(false);
            info.AddValue("SignerKey", rsaParameters);
        }

        #endregion
    }
}
