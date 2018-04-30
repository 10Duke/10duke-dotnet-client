using System;
using System.Runtime.Serialization;

namespace Tenduke.Client.Config
{
    /// <summary>
    /// Base class OAuth configuration objects.
    /// </summary>
    [Serializable]
    public abstract class OAuthConfig : IOAuthConfig, ISerializable
    {
        #region Properties

        /// <summary>
        /// The OAuth 2.0 client id.
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// The OAuth 2.0 scope. If using OpenID Connect, <c>openid</c> scope value must be included.
        /// </summary>
        /// <example>openid profile email</example>
        public string Scope { get; set; }

        /// <summary>
        /// Uri of the OpenID Connect userinfo endpoint of the 10Duke Entitlement service.
        /// </summary>
        public string UserInfoUri { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthConfig"/> class.
        /// </summary>
        public OAuthConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OAuthConfig"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected OAuthConfig(SerializationInfo info, StreamingContext context)
        {
            ClientID = info.GetString("ClientID");
            Scope = info.GetString("Scope");
            UserInfoUri = info.GetString("UserInfoUri");
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets data for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ClientID", ClientID);
            info.AddValue("Scope", Scope);
            info.AddValue("UserInfoUri", UserInfoUri);
        }

        #endregion
    }
}
