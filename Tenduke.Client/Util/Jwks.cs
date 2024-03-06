using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// List of Json Web Keys
    /// </summary>
    /// <remarks>
    /// Initializes a new instance.
    /// </remarks>
    /// <param name="keys">List of keys.</param>
    internal class Jwks(IEnumerable<JsonWebKey> keys)
    {
        /// <summary>
        /// List of keys.
        /// </summary>
        public IEnumerable<JsonWebKey> Keys { get; } = keys;
    }


}
