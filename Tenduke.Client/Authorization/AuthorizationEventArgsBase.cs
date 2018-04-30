using System;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Base class for event arguments communicating phases of authorization process.
    /// </summary>
    public class AuthorizationEventArgsBase
    {
        /// <summary>
        /// Arguments for the authorization operation.
        /// </summary>
        public AuthorizationArgs AuthzArgs { get; set; }

        /// <summary>
        /// The full Uri for the authorization request that 
        /// </summary>
        public Uri AuthzUri { get; set; }
    }
}
