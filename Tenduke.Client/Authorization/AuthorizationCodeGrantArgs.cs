using System;
using System.Security.Cryptography;
using System.Text;
using Tenduke.Client.Util;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Arguments for authorization operations of <see cref="AuthorizationCodeGrant"/>.
    /// </summary>
    public class AuthorizationCodeGrantArgs : BrowserBasedAuthorizationArgs
    {
        /// <summary>
        /// PKCE (Proof Key for Code Exchange) code verifier. If set, PKCE is used for securing
        /// the Authorization Code Grant flow. If not set, <code>client_secret</code> will be
        /// required for obtaining access token.
        /// </summary>
        public string CodeVerifier { get; set; }

        /// <summary>
        /// Generates a new PKCE (Proof Key for Code Exchange) code verifier and sets it as
        /// value of <see cref="CodeVerifier"/>, thus enabling PKCE in the Authorization Code
        /// Grant flow.
        /// </summary>
        /// <returns>Returns this object</returns>
        public AuthorizationCodeGrantArgs WithNewCodeVerifier()
        {
            CodeVerifier = GeneratePKCECodeVerifier();
            return this;
        }

        /// <summary>
        /// Computes PKCE (Proof Key for Code Exchange) code challenge from <see cref="CodeVerifier"/>.
        /// <see cref="CodeVerifier"/> must be set when calling this method.
        /// </summary>
        /// <returns>The PKCE code challenge value respective to <see cref="CodeVerifier"/>.</returns>
        public string ComputeCodeChallenge()
        {
            if (CodeVerifier == null)
            {
                throw new InvalidOperationException("CodeVerifier must be set");
            }

#if NET5_0_OR_GREATER
            var challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(CodeVerifier));
            return Base64Url.Encode(challengeBytes);
#else
            using var sha256 = SHA256.Create();
            var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(CodeVerifier));
            return Base64Url.Encode(challengeBytes);
#endif
        }

        /// <summary>
        /// Generates a new PKCE code verifier.
        /// </summary>
        /// <returns>The code verifier string</returns>
        private string GeneratePKCECodeVerifier()
        {
            var random = RandomNumberGenerator.Create();
            var bytes = new byte[32];
            random.GetBytes(bytes);
            return Base64Url.Encode(bytes);
        }
    }
}
