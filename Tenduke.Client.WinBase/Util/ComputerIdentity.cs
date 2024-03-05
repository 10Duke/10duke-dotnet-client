using Microsoft.Win32;
using System;
using System.Collections;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Tenduke.Client.Util;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// <para>Utilities for getting different pieces of information that may be used for identifying
    /// the computer. It is up to developer to decide how to identify the computer, with or
    /// without using these utilities.</para>
    /// <para>These utilities are specific to Windows.</para>
    /// </summary>
    public static class ComputerIdentity
    {
        #region Private fields

        /// <summary>
        /// Salt that is always used for computing the computer id salt. Application specific salt can
        /// be specified in addition to this salt.
        /// </summary>
        private static readonly string ComputerIdSalt = "Û%Ý×õ ˜ä£œ*A’ç,5Ç";

        #endregion

        #region Public constants

        /// <summary>
        /// Registry key path to current Windows version registry keys, under <c>HKEY_LOCAL_MACHINE</c>.
        /// </summary>
        public static readonly string REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion";

        /// <summary>
        /// Registry key of digital product id (32-bit systems), in <see cref="REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION"/>.
        /// </summary>
        public static readonly string REGISTRY_KEY_DIGITAL_PRODUCT_ID = "DigitalProductId";

        /// <summary>
        /// Registry key of digital product id (64-bit systems), in <see cref="REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION"/>.
        /// </summary>
        public static readonly string REGISTRY_KEY_DIGITAL_PRODUCT_ID_4 = "DigitalProductId4";

        /// <summary>
        /// Registry key of product id, in <see cref="REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION"/>.
        /// </summary>
        public static readonly string REGISTRY_KEY_PRODUCT_ID = "ProductId";

        /// <summary>
        /// WMI class name for <c>Win32_BaseBoard</c>.
        /// </summary>
        public static readonly string MANAGEMENT_INFO_BASEBOARD = "Win32_BaseBoard";

        /// <summary>
        /// WMI class name for <c>Win32_ComputerSystemProduct</c>.
        /// </summary>
        public static readonly string MANAGEMENT_INFO_COMPUTER_SYSTEM_PRODUCT = "Win32_ComputerSystemProduct";

        #endregion

        #region ComputerIdentifier enumeration

        /// <summary>
        /// Enumeration of computer identifiers supported by this class.
        /// </summary>
        public enum ComputerIdentifier
        {
            /// <summary>
            /// <c>SerialNumber</c> value of the <see cref="MANAGEMENT_INFO_BASEBOARD"/> WMI class.
            /// </summary>
            BaseboardSerialNumber,

            /// <summary>
            /// <c>UUID</c> value of the <see cref="MANAGEMENT_INFO_COMPUTER_SYSTEM_PRODUCT"/> WMI class.
            /// </summary>
            ComputerSystemProductUuid,

            /// <summary>
            /// Architecture dependent digital product id value of the <see cref="REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION"/> registry key path.
            /// </summary>
            WindowsDigitalProductId,

            /// <summary>
            /// <see cref="REGISTRY_KEY_PRODUCT_ID"/> value of the <see cref="REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION"/> registry key path.
            /// </summary>
            WindowsProductId
        }

        #endregion

        #region ComputerIdentifier enumeration

        /// <summary>
        /// Hash algorithm to use for building the computer identifier.
        /// </summary>
        public enum HashAlg
        {
            /// <summary>
            /// SHA1
            /// </summary>
            SHA1,

            /// <summary>
            /// SHA256
            /// </summary>
            SHA256,
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a hash of selected computer identifiers. This overload uses <see cref="HashAlg.SHA1"/> for computing the hash.
        /// </summary>
        /// <param name="customId">You own custom value to as a component of the computer identifier, may represent a single
        /// identifier or multiple identifiers concatenated. <c>null</c> for no custom / caller-specified computer id.</param>
        /// <param name="salt">Application specific salt for computing the hash, or <c>null</c> for no application-specific salt.</param>
        /// <param name="identifyBy"><see cref="ComputerIdentifier"/> values to use as components for the computer identifier.</param>
        /// <returns>The computer identifier hash as a URL safe string.</returns>
        public static string BuildComputerId(string customId, string salt, params ComputerIdentifier[] identifyBy)
        {
            return BuildComputerId(customId, salt, HashAlg.SHA256, identifyBy);
        }

        /// <summary>
        /// Builds a hash of selected computer identifiers.
        /// </summary>
        /// <param name="customId">You own custom value to as a component of the computer identifier, may represent a single
        /// identifier or multiple identifiers concatenated. <c>null</c> for no custom / caller-specified computer id.</param>
        /// <param name="salt">Application specific salt for computing the hash, or <c>null</c> for no application-specific salt.</param>
        /// <param name="identifyBy"><see cref="ComputerIdentifier"/> values to use as components for the computer identifier.</param>
        /// <returns>The computer identifier hash as a URL safe string.</returns>
        public static string BuildComputerId(string customId, string salt, HashAlg hashAlg, params ComputerIdentifier[] identifyBy)
        {
            var baseString = new StringBuilder();
            baseString.Append(ComputerIdentity.ComputerIdSalt);

            if (!string.IsNullOrEmpty(customId))
            {
                baseString.Append(customId);
            }

            foreach (var requestedIdentifier in identifyBy)
            {
                switch (requestedIdentifier)
                {
                    case ComputerIdentifier.BaseboardSerialNumber:
                        baseString.Append(GetBaseboardSerialNumber());
                        break;
                    case ComputerIdentifier.ComputerSystemProductUuid:
                        baseString.Append(GetComputerSystemProductUuid());
                        break;
                    case ComputerIdentifier.WindowsDigitalProductId:
                        baseString.Append(GetWindowsDigitalProductId());
                        break;
                    case ComputerIdentifier.WindowsProductId:
                        baseString.Append(GetWindowsProductId());
                        break;
                    default:
                        throw new NotSupportedException(string.Format("Computer identity value {0} not supported", requestedIdentifier.ToString()));
                }
            }

            if (!string.IsNullOrEmpty(salt))
            {
                baseString.Append(salt);
            }

            return ComputeHash(baseString.ToString(), hashAlg);
        }

        /// <summary>
        /// Gets the <c>UUID</c> value of the <see cref="MANAGEMENT_INFO_COMPUTER_SYSTEM_PRODUCT"/> WMI class.
        /// </summary>
        /// <returns>The UUID value as a string, or <c>null</c> if not available.</returns>
        public static string GetComputerSystemProductUuid()
        {
            using var managementClass = new ManagementClass(MANAGEMENT_INFO_COMPUTER_SYSTEM_PRODUCT);
            using var managementObjs = managementClass.GetInstances();
            string retValue = null;
            foreach (var managementObj in managementObjs)
            {
                retValue = (string)managementObj.Properties["UUID"]?.Value;
                if (!string.IsNullOrEmpty(retValue) && !Guid.Empty.ToString().Equals(retValue))
                {
                    break;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Gets the <c>SerialNumber</c> value of the <see cref="MANAGEMENT_INFO_BASEBOARD"/> WMI class.
        /// </summary>
        /// <returns>The motherboard serial number.</returns>
        public static string GetBaseboardSerialNumber()
        {
            using var managementClass = new ManagementClass(MANAGEMENT_INFO_BASEBOARD);
            using var managementObjs = managementClass.GetInstances();
            string retValue = null;
            foreach (var managementObj in managementObjs)
            {
                retValue = (string)managementObj.Properties["SerialNumber"]?.Value;
                if (!string.IsNullOrEmpty(retValue))
                {
                    break;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Gets value of the Windows <see cref="REGISTRY_KEY_PRODUCT_ID"/> from the registry.
        /// </summary>
        /// <returns>The product id.</returns>
        public static string GetWindowsProductId()
        {
            using var baseKey =
                Environment.Is64BitOperatingSystem
                ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            using var winCurrentVersionKeyPath = baseKey.OpenSubKey(REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION);
            return winCurrentVersionKeyPath.GetValue(REGISTRY_KEY_PRODUCT_ID) as string;
        }

        /// <summary>
        /// Gets value of the Windows <see cref="REGISTRY_KEY_DIGITAL_PRODUCT_ID"/> or <see cref="REGISTRY_KEY_DIGITAL_PRODUCT_ID_4"/> from the registry,
        /// depending on the architecture.
        /// </summary>
        /// <returns>The digital product id.</returns>
        public static string GetWindowsDigitalProductId()
        {
            using var baseKey =
                Environment.Is64BitOperatingSystem
                ? RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                : RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            using var winCurrentVersionKeyPath = baseKey.OpenSubKey(REGISTRY_KEY_PATH_WINDOWS_CURRENT_VERSION);
            var key = Environment.Is64BitOperatingSystem ? REGISTRY_KEY_DIGITAL_PRODUCT_ID_4 : REGISTRY_KEY_DIGITAL_PRODUCT_ID;
            var digitalProductId = winCurrentVersionKeyPath.GetValue(key) as byte[];
            return DecodeFromDigitalProductId(digitalProductId);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Decodes product key from Microsoft digital product id.
        /// </summary>
        /// <param name="digitalProductId">The Microsoft digital product id.</param>
        /// <returns>Product key decoded from the Microsoft digital product id.</returns>
        private static string DecodeFromDigitalProductId(byte[] digitalProductId)
        {
            // Offset of first byte of encoded product key in 'DigitalProductIdxxx" REG_BINARY value. Offset = 34H.
            const int KeyStartIndex = 52;

            // Offset of last byte of encoded product key in 'DigitalProductIdxxx" REG_BINARY value. Offset = 43H.
            const int KeyEndIndex = KeyStartIndex + 15;

            // Possible alpha-numeric characters in product key.
            char[] digits =
                [
                    'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R',
                    'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9'
                ];

            // Length of decoded product key
            const int DecodeLength = 29;

            // Length of decoded product key in byte-form. Each byte represents 2 chars.
            const int DecodeStringLength = 15;

            // Array of containing the decoded product key.
            char[] decodedChars = new char[DecodeLength];

            // Extract byte 52 to 67 inclusive.
            ArrayList hexPid = [];
            for (int i = KeyStartIndex; i <= KeyEndIndex; i++)
            {
                hexPid.Add(digitalProductId[i]);
            }

            for (int i = DecodeLength - 1; i >= 0; i--)
            {
                // Every sixth char is a separator.
                if ((i + 1) % 6 == 0)
                {
                    decodedChars[i] = '-';
                }
                else
                {
                    // Do the actual decoding.
                    int digitMapIndex = 0;
                    for (int j = DecodeStringLength - 1; j >= 0; j--)
                    {
                        int byteValue = (digitMapIndex << 8) | (byte)hexPid[j];
                        hexPid[j] = (byte)(byteValue / 24);
                        digitMapIndex = byteValue % 24;
                        decodedChars[i] = digits[digitMapIndex];
                    }
                }
            }

            return new string(decodedChars);
        }

        /// <summary>
        /// Computes the computer id hash.
        /// </summary>
        /// <param name="baseString">Base string built by appending one or more identifier elements.</param>
        /// <param name="hashAlg">The <see cref="HashAlg"/> to use.</param>
        /// <returns>The computer identifier hash as a URL safe string.</returns>
        /// <exception cref="NotSupportedException">Thrown if the given <paramref name="hashAlg"/> is not supported.</exception>
        private static string ComputeHash(string baseString, HashAlg hashAlg)
        {
            byte[] hash;
            if (hashAlg == HashAlg.SHA1)
            {
                using var hashAlgorithm = SHA1.Create();
                hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(baseString));
            }
            else if (hashAlg == HashAlg.SHA256)
            {
                using var sha256 = SHA256.Create();
                hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(baseString));
            }
            else
            {
                var message = string.Format("Hash algorithm {0} not supported", hashAlg);
                throw new NotSupportedException(message);
            }

            return Base64Url.Encode(hash);
        }
        #endregion
    }
}
