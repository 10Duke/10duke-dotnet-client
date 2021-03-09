using System;
using System.IO;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.Util;
using Tenduke.Client.WinBase;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Object that works with an <see cref="EntClient"/> instance for reading and writing <see cref="EntClient.Authorization"/>
    /// by binary serialization.
    /// </summary>
    public class EntClientAuthorizationSerializer<C> : AuthorizationSerializer<C, IAuthorizationCodeGrantConfig>
                where C : BaseWinClient<C>
    {
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

        #endregion
    }
}
