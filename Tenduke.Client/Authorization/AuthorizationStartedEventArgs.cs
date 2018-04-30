using Tenduke.Client.Config;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Arguments for the authorization start event.
    /// </summary>
    /// <typeparam name="O">OAuth 2.0 configuration object type.</typeparam>
    public class AuthorizationStartedEventArgs<O> where O : OAuthConfig
    {
        /// <summary>
        /// The OAuth 2.0 configuration used for the authorization process.
        /// </summary>
        public O OAuthConfig { get; set; }
    }
}
