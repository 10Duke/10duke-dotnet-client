using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// Cryptographic / security utilities.
    /// </summary>
    public static class CryptoUtil
    {
        /// <summary>
        /// Generates a random nonce string that can be used e.g. as the OpenID connect nonce.
        /// </summary>
        /// <param name="length">Number of characters in the generated string.</param>
        /// <param name="charsToUse">Characters that may be used in the generated string. Default is the character set used in Base64 URL-safe encoding,
        /// i.e. a-z, A-Z, 0-9, - (minus) and _ (underscore).</param>
        /// <returns>The random nonce string.</returns>
        public static string GenerateNonce(int length = 16, string charsToUse = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-_")
        {
            using (var crypto = new RNGCryptoServiceProvider())
            {
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
                        if (tmpBuffer == null)
                        {
                            tmpBuffer = new byte[1];
                        }
                        crypto.GetBytes(tmpBuffer);
                        randomByte = tmpBuffer[0];
                    }

                    resultChars[i] = charsToUse[randomByte % charsToUse.Length];
                }

                return new string(resultChars);
            }
        }

        /// <summary>
        /// Reads an RSA public key from PEM / PKCS#1 format.
        /// </summary>
        /// <param name="publicKeyPkcs1Pem">String representation of the public key, in PEM / PKCS#1 format.</param>
        /// <returns><see cref="RSACryptoServiceProvider"/> object representing the RSA public key.</returns>
        public static RSACryptoServiceProvider ReadRsaPublicKey(string publicKeyPkcs1Pem)
        {
            var encodedKey = publicKeyPkcs1Pem
                .Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
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
            byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            byte[] seq = new byte[15];
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            MemoryStream mem = new MemoryStream(publicKeyPkcs1);
            BinaryReader binr = new BinaryReader(mem);    //wrap Memory Stream with BinaryReader for easy reading
            byte bt = 0;
            ushort twobytes = 0;

            try
            {

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8230)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                seq = binr.ReadBytes(15);       //read the Sequence OID
                if (!seq.SequenceEqual(SeqOID))    //make sure Sequence for OID is correct
                    return null;

                twobytes = binr.ReadUInt16();
                if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                    binr.ReadByte();    //advance 1 byte
                else if (twobytes == 0x8203)
                    binr.ReadInt16();   //advance 2 bytes
                else
                    return null;

                bt = binr.ReadByte();
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
                byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
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
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
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
    }
}
