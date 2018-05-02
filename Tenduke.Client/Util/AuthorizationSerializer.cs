using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// Object that works with an <see cref="TendukeClient"/> instance for reading and writing <see cref="BaseClient.Authorization"/>
    /// by binary serialization.
    /// </summary>
    public class AuthorizationSerializer<C, A> where A: IOAuthConfig where C : BaseClient<C, A>
    {
        #region Properties

        /// <summary>
        /// The <see cref="TendukeClient"/> for which <see cref="AuthorizationInfo"/> is serialized or deserialized.
        /// </summary>
        public C TendukeClient { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reads <see cref="BaseClient.Authorization"/> of the <see cref="TendukeClient"/> property object
        /// and serializes the authorization object.
        /// </summary>
        /// <param name="stream">The stream for writing the binary serialized authorization.
        /// Caller of this method is responsible for closing the stream after calling this method.</param>
        public void ReadAuthorization(Stream stream)
        {
            var authorization = AssertTendukeClientAuthorization();
            SerializeAuthorization(authorization, stream);
        }

        /// <summary>
        /// Reads <see cref="BaseClient.Authorization"/> of the <see cref="TendukeClient"/> property object
        /// and serializes the authorization object into a byte array.
        /// </summary>
        /// <returns>Byte array representing the serialized authorization object.</returns>
        public byte[] ReadAuthorization()
        {
            var authorization = AssertTendukeClientAuthorization();
            return SerializeAuthorization(authorization);
        }

        /// <summary>
        /// Reads <see cref="BaseClient.Authorization"/> of the <see cref="TendukeClient"/> property object
        /// and serializes the authorization object into a Base 64 encoded string.
        /// </summary>
        /// <returns>Base 64 encoded string representing the serialized authorization object.</returns>
        public string ReadAuthorizationToBase64()
        {
            var authorization = AssertTendukeClientAuthorization();
            return SerializeAuthorizationToBase64(authorization);
        }

        /// <summary>
        /// Deserializes an authorization object and sets <see cref="BaseClient.Authorization"/> of the
        /// <see cref="TendukeClient"/> property object.
        /// </summary>
        /// <param name="stream">Stream for reading the serialized <see cref="AuthorizationInfo"/> object.
        /// Caller of this method is responsible for closing the stream after calling this method.</param>
        public void WriteAuthorization(Stream stream)
        {
            var client = AssertTendukeClient();
            var authorization = DeserializeAuthorization(stream);
            client.Authorization = authorization;
        }

        /// <summary>
        /// Deserializes an authorization object and sets <see cref="BaseClient.Authorization"/> of the
        /// <see cref="TendukeClient"/> property object.
        /// </summary>
        /// <param name="serialized">Byte array representing the serialized <see cref="AuthorizationInfo"/> object.</param>
        public void WriteAuthorization(byte[] serialized)
        {
            var client = AssertTendukeClient();
            var authorization = DeserializeAuthorization(serialized);
            client.Authorization = authorization;
        }

        /// <summary>
        /// Deserializes an authorization object and sets <see cref="BaseClient.Authorization"/> of the
        /// <see cref="TendukeClient"/> property object.
        /// </summary>
        /// <param name="serialized">Base64 encoded string representing the serialized <see cref="AuthorizationInfo"/> object.</param>
        public void WriteAuthorizationFromBase64(string serialized)
        {
            var client = AssertTendukeClient();
            var authorization = DeserializeAuthorizationFromBase64(serialized);
            client.Authorization = authorization;
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
        /// Throws <see cref="InvalidOperationException"/> if <see cref="BaseClient.Authorization"/> is not set.
        /// </summary>
        /// <returns>Returns the <see cref="AuthorizationInfo"/> object describing authorization that the
        /// <see cref="TendukeClient"/> has, verified to be not <c>null</c>.</returns>
        private AuthorizationInfo AssertTendukeClientAuthorization()
        {
            var client = AssertTendukeClient();
            if (client.Authorization == null)
            {
                throw new InvalidOperationException("Authorization must be set on the TendukeClient");
            }

            return client.Authorization;
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> if <see cref="TendukeClient"/> is not set.
        /// </summary>
        /// <returns>Returns the <see cref="TendukeClient"/> property value that is verified to be not <c>null</c>.</returns>
        private C AssertTendukeClient()
        {
            if (TendukeClient == null)
            {
                throw new InvalidOperationException("TendukeClient must be set");
            }

            return TendukeClient;
        }

        #endregion
    }
}
