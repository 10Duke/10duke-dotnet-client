using System;
using System.Runtime.Serialization;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Configuration for using the OAuth 2.0 Authorization Code Grant flow for
    /// communicating with the 10Duke Entitlement service.
    /// </summary>
    [Serializable]
    public class AuthorizationCodeGrantConfig : BrowserBasedAuthorizationConfig, IAuthorizationCodeGrantConfig
    {
        #region Properties

        /// <summary>
        /// OAuth 2.0 client secret. Please note that this is ignored if
        /// <see cref="UsePkce"/> is <c>true</c>.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Uri of the OAuth 2.0 token endpoint of the 10Duke Entitlement service.
        /// </summary>
        public string TokenUri { get; set; }

        /// <summary>
        /// Indicates if PKCE (Proof Key for Code Exchange) is used. Please note that
        /// if <c>true</c>, <see cref="ClientSecret"/> is ignored.
        /// </summary>
        public bool UsePkce { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCodeGrantConfig"/> class.
        /// </summary>
        public AuthorizationCodeGrantConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizationCodeGrantConfig"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected AuthorizationCodeGrantConfig(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ClientSecret = info.GetString("ClientSecret");
            TokenUri = info.GetString("TokenUri");
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
            info.AddValue("ClientSecret", ClientSecret);
            info.AddValue("TokenUri", TokenUri);
        }

        #endregion
    }
}
