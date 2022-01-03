using System;
using System.Runtime.Serialization;
using Tenduke.Client.Config;

namespace Tenduke.Client.DefaultBrowser.Config
{
    /// <summary>
    /// Configuration for using the OAuth 2.0 Authorization Code Grant flow for
    /// communicating with the 10Duke Entitlement service, using the OS default
    /// browser for user interaction.
    /// </summary>
    [Serializable]
    public class DefaultBrowserAuthorizationCodeGrantConfig : AuthorizationCodeGrantConfig, IDefaultBrowserAuthorizationCodeGrantConfig
    {
        /// <summary>
        /// Default timeout for waiting for response of the authorization / authentication process.
        /// If not specified, default timeout is used.
        /// </summary>
        public int? ResponseTimeout { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBrowserAuthorizationCodeGrantConfig"/> class.
        /// </summary>
        public DefaultBrowserAuthorizationCodeGrantConfig()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBrowserAuthorizationCodeGrantConfig"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected DefaultBrowserAuthorizationCodeGrantConfig(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ResponseTimeout = info.GetValue("ResponseTimeout", typeof(int?)) as int?;
        }

        /// <summary>
        /// Gets data for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ResponseTimeout", ResponseTimeout);
        }
    }
}
