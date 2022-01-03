using System.Security.Cryptography;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// RSA key used for token signing.
    /// </summary>
    public class RSASigningKey
    {
        /// <summary>
        /// The key id, or <c>null</c> if unknown.
        /// </summary>
        public string KeyId { get; set; }

        /// <summary>
        /// The RSA key.
        /// </summary>
        public RSACryptoServiceProvider RSAKey { get; set; }
    }
}
