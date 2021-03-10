using Tenduke.Client.Config;

namespace Tenduke.Client.DefaultBrowser.Config
{
    /// <summary>
    /// Configuration for using the OAuth 2.0 Authorization Code Grant flow for
    /// communicating with the 10Duke Entitlement service, using the OS default
    /// browser for user interaction.
    /// </summary>
    public class DefaultBrowserAuthorizationCodeGrantConfig : AuthorizationCodeGrantConfig, IDefaultBrowserAuthorizationCodeGrantConfig
    {
        /// <summary>
        /// Default timeout for waiting for response of the authorization / authentication process.
        /// If not specified, default timeout is used.
        /// </summary>
        public int? ResponseTimeout { get; set; }
    }
}
