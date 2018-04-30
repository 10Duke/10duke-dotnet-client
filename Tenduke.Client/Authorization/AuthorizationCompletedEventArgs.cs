namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Arguments for the authorization completed event.
    /// </summary>
    public class AuthorizationCompletedEventArgs
    {
        /// <summary>
        /// The OAuth 2.0 Access Token response.
        /// </summary>
        public AccessTokenResponse AccessTokenResponse { get; set; }
    }
}
