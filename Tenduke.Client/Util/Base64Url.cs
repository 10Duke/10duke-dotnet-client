using System;
using System.Collections.Generic;
using System.Text;

namespace Tenduke.Client.Util
{
    public static class Base64Url
    {
        /// <summary>
        /// Encodes the given data using URL-safe Base64 encoding.
        /// </summary>
        /// <param name="data">The bytes to encode.</param>
        /// <returns>The encoded data.</returns>
        public static string Encode(byte[] data)
        {
            return Convert.ToBase64String(data).TrimEnd('=').Replace("+", "-").Replace("/", "_");
        }

        /// <summary>
        /// Decodes the given URL-safe Base64 encoded string.
        /// </summary>
        /// <param name="encoded">The string to decode.</param>
        /// <returns>The decoded bytes.</returns>
        public static byte[] Decode(string encoded)
        {
            var urlSafeWithoutPadding = encoded.Replace("-", "+").Replace("_", "/");
            var urlSafe = urlSafeWithoutPadding.PadRight(urlSafeWithoutPadding.Length + (4 - urlSafeWithoutPadding.Length % 4) % 4, '=');
            return Convert.FromBase64String(urlSafe);
        }

        /// <summary>
        /// Decodes the given URL-safe Base64 encoded string and interprets the decoded
        /// value as an UTF-8 string.
        /// </summary>
        /// <param name="encoded">The string to decode.</param>
        /// <returns>The decoded string value.</returns>
        public static string DecodeString(string encoded)
        {
            var decoded = Decode(encoded);
            return Encoding.UTF8.GetString(decoded);
        }
    }
}
