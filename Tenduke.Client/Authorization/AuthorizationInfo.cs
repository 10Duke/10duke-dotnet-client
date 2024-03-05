using System;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Base class for authorization implementations, containing the authorization result data.
    /// </summary>
    [Serializable]
    public abstract class AuthorizationInfo
    {
        /// <summary>
        /// OAuth 2.0 Access Token response received from the 10Duke Identity and Entitlement service,
        /// or <c>null</c> if no access token currently obtained.
        /// </summary>
        public AccessTokenResponse AccessTokenResponse { get; set; }

        /// <summary>
        /// Error code received from the 10Duke Identity and Entitlement service, or <c>null</c> if there is no error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Error description received from the 10Duke Identity and Entitlement service.
        /// The error description may be given in the case that <see cref="Error"/> is not <c>null</c>.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Error Uri for additional error information, received from the 10Duke Identity and Entitlement service.
        /// The error Uri may be given in the case that <see cref="Error"/> is not <c>null</c>.
        /// </summary>
        public string ErrorUri { get; set; }

        /// <summary>
        /// Timestamp when this authorization info has been received from the 10Duke Identity and Entitlement service.
        /// </summary>
        public DateTime? Received { get; set; }
    }
}
