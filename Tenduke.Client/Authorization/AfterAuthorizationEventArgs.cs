using System.Collections.Specialized;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Arguments for the event occurring just after executing request to the authorization endpoint has been
    /// completed.
    /// </summary>
    public class AfterAuthorizationEventArgs : AuthorizationEventArgsBase
    {
        /// <summary>
        /// Response parameters received in the response from the server.
        /// </summary>
        public NameValueCollection ResponseParameters { get; set; }
    }
}
