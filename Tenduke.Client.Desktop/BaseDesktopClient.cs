using Tenduke.Client.Authorization;
using Tenduke.Client.Config;

namespace Tenduke.Client.Desktop
{
    /// <summary>
    /// Base class for desktop clients working against the 10Duke Entitlement service and using
    /// the CefSharp embedded browser for user interaction.
    /// </summary>
    public class BaseDesktopClient<C> : BaseClient<C, IAuthorizationCodeGrantConfig> where C : BaseDesktopClient<C>
    {
        #region Properties

        /// <summary>
        /// Authorization process result information received from the 10Duke Entitlement service.
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        #endregion
    }
}
