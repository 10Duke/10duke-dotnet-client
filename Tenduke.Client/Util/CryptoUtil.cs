using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// Cryptographic / security utilities.
    /// </summary>
    public static class CryptoUtil
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        #region Methods

        /// <summary>
        /// Generates a random nonce string that can be used e.g. as the OpenID connect nonce.
        /// </summary>
        /// <param name="length">Number of characters in the generated string.</param>
        /// <param name="charsToUse">Characters that may be used in the generated string. Default is the character set used in Base64 URL-safe encoding,
        /// i.e. a-z, A-Z, 0-9, - (minus) and _ (underscore).</param>
        /// <returns>The random nonce string.</returns>
        public static string GenerateNonce(int length = 16, string charsToUse = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_")
        {
#if NET5_0_OR_GREATER
            int maxExclusive = charsToUse.Length;
            var resultChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                var index = RandomNumberGenerator.GetInt32(maxExclusive);
                resultChars[i] = charsToUse[index];
            }
            return new string(resultChars);
#else
            using var crypto = new RNGCryptoServiceProvider();
            var data = new byte[length];
            int maxAllowedRandom = byte.MaxValue - ((byte.MaxValue + 1) % charsToUse.Length);
            byte[] tmpBuffer = null;
            crypto.GetBytes(data);
            var resultChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                byte randomByte = data[i];

                // Ensure random distribution
                while (randomByte > maxAllowedRandom)
                {
                    tmpBuffer ??= new byte[1];
                    crypto.GetBytes(tmpBuffer);
                    randomByte = tmpBuffer[0];
                }

                resultChars[i] = charsToUse[randomByte % charsToUse.Length];
            }

            return new string(resultChars);
#endif
        }

        /// <summary>
        /// Reads an RSA public key from PEM / PKCS#1 format, or from JWKS URL. In practice there will
        /// always be just one currently valid public key.
        /// </summary>
        /// <param name="publicKeyPkcs1PemOrJwksUri">String representation of the public key, in PEM / PKCS#1 format,
        /// or URL of a JWKS endpoint.</param>
        /// <returns><see cref="RSACryptoServiceProvider"/> object representing the RSA public key.</returns>
        public async static Task<RSASigningKey> ReadFirstRsaPublicKey(
            string publicKeyPkcs1PemOrJwksUri,
            HttpClient httpClient)
        {
            var keys = await ReadRsaPublicKeys(publicKeyPkcs1PemOrJwksUri, httpClient);
            return keys[0];
        }

        /// <summary>
        /// Reads RSA public keys from PEM / PKCS#1 format, or from JWKS URL.
        /// </summary>
        /// <param name="publicKeyPkcs1PemOrJwksUri">String representation of the public key, in PEM / PKCS#1 format,
        /// or URL of a JWKS endpoint.</param>
        /// <returns>Collection of <see cref="RSACryptoServiceProvider"/> objects.</returns>
        public async static Task<IList<RSASigningKey>> ReadRsaPublicKeys(
            string publicKeyPkcs1PemOrJwksUri,
            HttpClient httpClient)
        {
            try
            {
                Uri jwksUrl = new(publicKeyPkcs1PemOrJwksUri);
                var request = new HttpRequestMessage()
                {
                    RequestUri = jwksUrl,
                    Method = HttpMethod.Get
                };

                request.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    NoCache = true,
                    NoStore = true
                };
                using var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var jwksResponseAsString = await response.Content.ReadAsStringAsync();
                var jwks = JsonSerializer.Deserialize<Jwks>(jwksResponseAsString, _jsonSerializerOptions);
                var retValue = new List<RSASigningKey>();
                foreach (var jwk in jwks.Keys)
                {
                    var rsaPubKey = JsonWebKeyPublicToRSA(jwk);
                    retValue.Add(new RSASigningKey { KeyId = jwk.KeyId, RSAKey = rsaPubKey });
                }

                return retValue;

            }
            catch (UriFormatException)
            {
                var encodedKey = publicKeyPkcs1PemOrJwksUri
                    .Replace("-----BEGIN PUBLIC KEY-----", "")
                    .Replace("-----END PUBLIC KEY-----", "")
                    .Replace("\r", "")
                    .Replace("\n", "");
                var rsaPubKey = ReadRsaPublicKey(Convert.FromBase64String(encodedKey));
                var signingKey = new RSASigningKey { KeyId = null, RSAKey = rsaPubKey };
                return [signingKey];
            }

        }

        /// <summary>
        /// Reads an RSA public key from PEM / PKCS#1 format, or from JWKS URL.
        /// </summary>
        /// <param name="publicKeyPkcs1PemOrJwksUri">String representation of the public key, in PEM / PKCS#1 format,
        /// or URL of a JWKS endpoint.</param>
        /// <returns><see cref="RSACryptoServiceProvider"/> object representing the RSA public key.</returns>
        public static RSACryptoServiceProvider ReadRsaPublicKey(string publicKeyPkcs1PemOrJwksUri)
        {
            var encodedKey = publicKeyPkcs1PemOrJwksUri
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\r", "")
                .Replace("\n", "");
            return ReadRsaPublicKey(Convert.FromBase64String(encodedKey));
        }

        /// <summary>
        /// Reads an RSA public key from PKCS#1 format.
        /// </summary>
        /// <param name="publicKeyPkcs1">The public key in PKCS#1 format.</param>
        /// <returns><see cref="RSACryptoServiceProvider"/> object representing the RSA public key.</returns>
        public static RSACryptoServiceProvider ReadRsaPublicKey(byte[] publicKeyPkcs1)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] SeqOID = [0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new(publicKeyPkcs1);
            BinaryReader binr = new(mem);    // wrap Memory Stream with BinaryReader for easy reading

            try
            {

                ushort twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                byte[] seq = binr.ReadBytes(15);
                if (!seq.SequenceEqual(SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                byte bt = binr.ReadByte();
                if (bt != 0x00)     //expect null byte next
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                twobytes = binr.ReadUInt16();
                byte lowbyte = 0x00;
                byte highbyte = 0x00;

                if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                    lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                else if (twobytes == 0x8202)
                {
                    highbyte = binr.ReadByte(); //advance 2 bytes
                    lowbyte = binr.ReadByte();
                }
                else
                    return null;
                byte[] modint = [lowbyte, highbyte, 0x00, 0x00];   // reverse byte order since asn.1 key uses big endian order
                int modsize = BitConverter.ToInt32(modint, 0);

                byte firstbyte = binr.ReadByte();
                binr.BaseStream.Seek(-1, SeekOrigin.Current);

                if (firstbyte == 0x00)
                {   //if first byte (highest order) of modulus is zero, don't include it
                    binr.ReadByte();    //skip this null byte
                    modsize -= 1;   //reduce modulus buffer size by 1
                }

                byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

                if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                    return null;
                int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                byte[] exponent = binr.ReadBytes(expbytes);

                // ------- create RSACryptoServiceProvider instance and initialize with public key -----
                RSACryptoServiceProvider rsa = new();
                rsa.ImportParameters(parameters: new RSAParameters
                {
                    Modulus = modulus,
                    Exponent = exponent
                });
                return rsa;
            }
            catch (Exception)
            {
                return null;
            }

            finally { binr.Close(); }
        }

        /// <summary>
        /// Converts a <see cref="JsonWebKey"/> with a public key to a <see cref="RSACryptoServiceProvider"/>.
        /// </summary>
        /// <param name="jwkPublic"><see cref="JsonWebKey"/> representing a public RSA key.</param>
        /// <returns><see cref="RSACryptoServiceProvider"/> with the public key.</returns>
        public static RSACryptoServiceProvider JsonWebKeyPublicToRSA(JsonWebKey jwkPublic)
        {
            var rsaParams = new RSAParameters()
            {
                Modulus = Base64Url.Decode(jwkPublic.N),
                Exponent = Base64Url.Decode(jwkPublic.E),
            };

            var retValue = new RSACryptoServiceProvider();
            retValue.ImportParameters(rsaParams);
            return retValue;
        }

#endregion
    }
}
