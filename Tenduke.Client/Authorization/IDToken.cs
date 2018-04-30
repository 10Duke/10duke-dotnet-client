using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// OpenID Connect ID token.
    /// </summary>
    public class IDToken
    {
        /// <summary>
        /// The ID token payload as a dynamic object.
        /// </summary>
        public dynamic ResponsePayload { get; set; }

        /// <summary>
        /// Access fields of the ID token.
        /// </summary>
        /// <param name="name">The ID token field name.</param>
        /// <returns>Value of the response field, or <c>null</c> if no such field found or if response object not populated.</returns>
        public object this[string name] => ResponsePayload?[name]?.Value;

        /// <summary>
        /// Parses the OpenID Connect ID token received from the 10Duke Entitlement Service and returns
        /// a new <see cref="IDToken"/> object encapsulating for accessing the ID Token fields.
        /// </summary>
        /// <param name="encodedToken">Raw, encoded ID Token, as received from the server.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying OpenID Connect ID token signature, if an ID token is present in the response.
        /// If <c>null</c>, no verification is done.</param>
        /// <returns><see cref="IDToken"/> object containing the parsed ID Token data.</returns>
        /// <exception cref="Jose.IntegrityException">Thrown if token signature verification fails.</exception>
        public static IDToken Parse(string encodedToken, RSA verifyWithKey)
        {
            string decoded;
            if (verifyWithKey == null)
            {
                decoded = Jose.JWT.Payload(encodedToken);
            }
            else
            {
                decoded = Jose.JWT.Decode(encodedToken, verifyWithKey);
            }
            dynamic json = JsonConvert.DeserializeObject(decoded);
            return new IDToken() { ResponsePayload = json };
        }
    }
}
