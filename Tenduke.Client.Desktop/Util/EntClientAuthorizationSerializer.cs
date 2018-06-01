using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Object that works with an <see cref="EntClient"/> instance for reading and writing <see cref="EntClient.Authorization"/>
    /// by binary serialization.
    /// </summary>
    public class EntClientAuthorizationSerializer<C> where C : BaseDesktopClient<C>
    {
        #region Properties

        /// <summary>
        /// The <see cref="BaseDesktopClient{C}"/> for which <see cref="AuthorizationInfo"/> is serialized or deserialized.
        /// </summary>
        public C EntClient { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reads <see cref="EntClient.Authorization"/> of the <see cref="EntClient"/> property object
        /// and serializes the authorization object.
        /// </summary>
        /// <param name="stream">The stream for writing the binary serialized authorization.
        /// Caller of this method is responsible for closing the stream after calling this method.</param>
        public void ReadAuthorization(Stream stream)
        {
            var authorization = AssertEntClientAuthorization();
            SerializeAuthorization(authorization, stream);
        }

        /// <summary>
        /// Reads <see cref="EntClient.Authorization"/> of the <see cref="EntClient"/> property object
        /// and serializes the authorization object into a byte array.
        /// </summary>
        /// <returns>Byte array representing the serialized authorization object.</returns>
        public byte[] ReadAuthorization()
        {
            var authorization = AssertEntClientAuthorization();
            return SerializeAuthorization(authorization);
        }

        /// <summary>
        /// Reads <see cref="EntClient.Authorization"/> of the <see cref="EntClient"/> property object
        /// and serializes the authorization object into a Base 64 encoded string.
        /// </summary>
        /// <returns>Base 64 encoded string representing the serialized authorization object.</returns>
        public string ReadAuthorizationToBase64()
        {
            var authorization = AssertEntClientAuthorization();
            return SerializeAuthorizationToBase64(authorization);
        }

        /// <summary>
        /// Deserializes an authorization object and sets <see cref="EntClient.Authorization"/> of the
        /// <see cref="EntClient"/> property object.
        /// </summary>
        /// <param name="stream">Stream for reading the serialized <see cref="AuthorizationInfo"/> object.
        /// Caller of this method is responsible for closing the stream after calling this method.</param>
        public void WriteAuthorization(Stream stream)
        {
            var entClient = AssertEntClient();
            var authorization = DeserializeAuthorization(stream);
            entClient.Authorization = authorization;
        }

        /// <summary>
        /// Deserializes an authorization object and sets <see cref="EntClient.Authorization"/> of the
        /// <see cref="EntClient"/> property object.
        /// </summary>
        /// <param name="serialized">Byte array representing the serialized <see cref="AuthorizationInfo"/> object.</param>
        public void WriteAuthorization(byte[] serialized)
        {
            var entClient = AssertEntClient();
            var authorization = DeserializeAuthorization(serialized);
            entClient.Authorization = authorization;
        }

        /// <summary>
        /// Deserializes an authorization object and sets <see cref="EntClient.Authorization"/> of the
        /// <see cref="EntClient"/> property object.
        /// </summary>
        /// <param name="serialized">Base64 encoded string representing the serialized <see cref="AuthorizationInfo"/> object.</param>
        public void WriteAuthorizationFromBase64(string serialized)
        {
            var entClient = AssertEntClient();
            var authorization = DeserializeAuthorizationFromBase64(serialized);
            entClient.Authorization = authorization;
        }

        /// <summary>
        /// Serializes <see cref="AuthorizationInfo"/> object into a binary stream.
        /// </summary>
        /// <param name="authorization">The <see cref="AuthorizationInfo"/> to serialize.</param>
        /// <param name="stream">The stream for writing the binary serialized authorization.
        /// Caller of this method is responsible for closing the stream after calling this method.</param>
        public static void SerializeAuthorization(AuthorizationInfo authorization, Stream stream)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, authorization);
        }

        /// <summary>
        /// Serializes <see cref="AuthorizationInfo"/> object into a byte array.
        /// </summary>
        /// <param name="authorization">The <see cref="AuthorizationInfo"/> to serialize.</param>
        /// <returns>Byte array representing the serialized authorization object.</returns>
        public static byte[] SerializeAuthorization(AuthorizationInfo authorization)
        {
            using (var stream = new MemoryStream())
            {
                SerializeAuthorization(authorization, stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Serializes <see cref="AuthorizationInfo"/> object into a Base 64 encoded string.
        /// </summary>
        /// <param name="authorization">The <see cref="AuthorizationInfo"/> to serialize.</param>
        /// <returns>Base 64 encoded string representing the serialized authorization object.</returns>
        public static string SerializeAuthorizationToBase64(AuthorizationInfo authorization)
        {
            byte[] serialized = SerializeAuthorization(authorization);
            return Convert.ToBase64String(serialized);
        }

        /// <summary>
        /// Deserializes <see cref="AuthorizationInfo"/>.
        /// </summary>
        /// <param name="stream">Stream for reading the serialized <see cref="AuthorizationInfo"/> object.
        /// Caller of this method is responsible for closing the stream after calling this method.</param>
        /// <returns>The deserialized <see cref="AuthorizationInfo"/>.</returns>
        public static AuthorizationInfo DeserializeAuthorization(Stream stream)
        {
            var formatter = new BinaryFormatter();
            var deserialized = formatter.Deserialize(stream);
            if (!(deserialized is AuthorizationInfo))
            {
                throw new InvalidOperationException("The serialized object does not represent a valid AuthorizationInfo");
            }

            return deserialized as AuthorizationInfo;
        }

        /// <summary>
        /// Deserializes <see cref="AuthorizationInfo"/>.
        /// </summary>
        /// <param name="serialized">Byte array representing the serialized <see cref="AuthorizationInfo"/> object.</param>
        /// <returns>The deserialized <see cref="AuthorizationInfo"/>.</returns>
        public static AuthorizationInfo DeserializeAuthorization(byte[] serialized)
        {
            using (var stream = new MemoryStream(serialized))
            {
                return DeserializeAuthorization(stream);
            }
        }

        /// <summary>
        /// Deserializes <see cref="AuthorizationInfo"/>.
        /// </summary>
        /// <param name="serialized">Base64 encoded string representing the serialized <see cref="AuthorizationInfo"/> object.</param>
        /// <returns>The deserialized <see cref="AuthorizationInfo"/>.</returns>
        public static AuthorizationInfo DeserializeAuthorizationFromBase64(string serialized)
        {
            var serializedBytes = Convert.FromBase64String(serialized);
            return DeserializeAuthorization(serializedBytes);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if <see cref="EntClient.Authorization"/> is not set.
        /// </summary>
        /// <returns>Returns the <see cref="AuthorizationInfo"/> object describing authorization that the
        /// <see cref="EntClient"/> has, verified to be not <c>null</c>.</returns>
        private AuthorizationInfo AssertEntClientAuthorization()
        {
            var entClient = AssertEntClient();
            if (entClient.Authorization == null)
            {
                throw new InvalidOperationException("Authorization must be set on the EntClient");
            }

            return entClient.Authorization;
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if <see cref="EntClient"/> is not set.
        /// </summary>
        /// <returns>Returns the <see cref="BaseDesktopClient{C}"/> property value that is verified to be not <c>null</c>.</returns>
        private C AssertEntClient()
        {
            if (EntClient == null)
            {
                throw new InvalidOperationException("EntClient must be set");
            }

            return EntClient;
        }

        #endregion
    }
}
