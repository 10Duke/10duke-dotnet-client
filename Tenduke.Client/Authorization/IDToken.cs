using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tenduke.Client.Util;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// OpenID Connect ID token.
    /// </summary>
    public class IDToken
    {
        [JsonPropertyName("iss")]
        public string Issuer { get; set; }
        [JsonPropertyName("sub")]
        public string Subject { get; set; }
        [JsonPropertyName("aud")]
        public string Audience { get; set; }
        [JsonPropertyName("exp")]
        public long Expiry { get; set; }
        [JsonPropertyName("iat")]
        public long IssuedAt { get; set; }
        [JsonPropertyName("auth_time")]
        public long AuthenticationTime { get; set; }
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }


        /// <summary>
        /// Parses the OpenID Connect ID token received from the 10Duke Entitlement Service and returns
        /// a new <see cref="IDToken"/> object encapsulating for accessing the ID Token fields.
        /// </summary>
        /// <param name="encodedToken">Raw, encoded ID Token, as received from the server.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying OpenID Connect ID token signature, if an ID token is present in the response.
        /// If <c>null</c>, no verification is done.</param>
        /// <returns><see cref="IDToken"/> object containing the parsed ID Token data.</returns>
        /// <exception cref="SecurityTokenInvalidSignatureException">Thrown if token signature verification fails.</exception>
        public static IDToken Parse(string encodedToken, RSA verifyWithKey)
        {
            var decoded = JwtUtil.ReadPayload(encodedToken, verifyWithKey);
            return JsonSerializer.Deserialize<IDToken>(decoded);
        }
    }
}
