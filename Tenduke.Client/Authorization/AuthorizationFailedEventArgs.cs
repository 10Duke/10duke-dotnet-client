using System;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Arguments for the authorization failed event.
    /// </summary>
    public class AuthorizationFailedEventArgs
    {
        /// <summary>
        /// Error code received from the 10Duke Entitlement service, or <c>null</c> if there is no error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Error description received from the 10Duke Entitlement service.
        /// The error description may be given in the case that <see cref="Error"/> is not <c>null</c>.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Error Uri for additional error information, received from the 10Duke Entitlement service.
        /// The error Uri may be given in the case that <see cref="Error"/> is not <c>null</c>.
        /// </summary>
        public string ErrorUri { get; set; }
    }
}
