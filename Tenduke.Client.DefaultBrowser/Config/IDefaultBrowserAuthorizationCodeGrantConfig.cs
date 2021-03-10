using Tenduke.Client.Config;

namespace Tenduke.Client.DefaultBrowser.Config
{
    /// <summary>
    /// Interface to be implemented by configuration for OAuth Authorization Code Grant flow
    /// using the OS default browser.
    /// </summary>
    public interface IDefaultBrowserAuthorizationCodeGrantConfig : IAuthorizationCodeGrantConfig
    {
        /// <summary>
        /// Default timeout for waiting for response of the authorization / authentication process.
        /// If not specified, default timeout is used.
        /// </summary>
        int? ResponseTimeout { get; }
    }
}
