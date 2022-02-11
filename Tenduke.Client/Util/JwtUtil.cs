using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Tenduke.Client.Util
{
    public static class JwtUtil
    {
        /// <summary>
        /// Validates token and reads the token payload.
        /// </summary>
        /// <param name="encodedToken">Raw, encoded ID Token, as received from the server.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying OpenID Connect ID token signature, if an ID token is present in the response.
        /// If <c>null</c>, no verification is done.</param>
        /// <returns>The token payload as a JSON string.</returns>
        /// <exception cref="SecurityTokenInvalidSignatureException">Thrown if token signature verification fails.</exception>
        public static string ReadPayload(string encodedToken, RSA verifyWithKey)
        {
            JsonWebToken token;
            if (verifyWithKey == null)
            {
                token = new JsonWebTokenHandler().ReadJsonWebToken(encodedToken);
            }
            else
            {
                var key = new RsaSecurityKey(verifyWithKey);
                var validationParams = new TokenValidationParameters()
                {
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
                var validationResult = new JsonWebTokenHandler().ValidateToken(encodedToken, validationParams);
                if (!validationResult.IsValid)
                {
                    if (validationResult.Exception != null)
                        throw validationResult.Exception;

                    throw new SecurityTokenInvalidSignatureException("Invalid ID token signature");
                }

                token = validationResult.SecurityToken as JsonWebToken;
            }

            return Base64Url.DecodeString(token.EncodedPayload);
        }
    }
}
