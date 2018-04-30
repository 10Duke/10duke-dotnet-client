namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Arguments for authorization operations of <see cref="BrowserBasedAuthorization{T, O}"/>.
    /// </summary>
    public class BrowserBasedAuthorizationArgs : AuthorizationArgs
    {
        /// <summary>
        /// The opaque state to maintain between the request an the callback.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// OpenID Connect nonce value, may be used if using OpenID Connect.
        /// </summary>
        public string Nonce { get; set; }
    }
}
