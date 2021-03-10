namespace Tenduke.Client.Config
{
    /// <summary>
    /// Interface to be implemented by configuration for OAuth Authorization Code Grant flow.
    /// </summary>
    public interface IAuthorizationCodeGrantConfig : IBrowserBasedAuthorizationConfig
    {
        #region Properties

        /// <summary>
        /// OAuth 2.0 client secret. Please note that this is ignored if
        /// <see cref="UsePkce"/> is <c>true</c>.
        /// </summary>
        string ClientSecret { get; }

        /// <summary>
        /// Uri of the OAuth 2.0 token endpoint of the 10Duke Entitlement service.
        /// </summary>
        string TokenUri { get; }

        /// <summary>
        /// Indicates if PKCE (Proof Key for Code Exchange) is used. Please note that
        /// if <c>true</c>, <see cref="ClientSecret"/> is ignored.
        /// </summary>
        bool UsePkce { get; }

        #endregion
    }
}
