using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Tenduke.Client.Util;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// OAuth 2.0 Access Token.
    /// </summary>
    [Serializable]
    public class AccessTokenResponse : ISerializable
    {
        #region Private fields

        /// <summary>
        /// OpenID Connect ID token parsed from the access token response.
        /// </summary>
        [NonSerialized]
        private IDToken parsedIdToken;

        #endregion

        #region Properties

        /// <summary>
        /// The whole access token response as a dynamic object.
        /// </summary>
        public dynamic ResponseObject { get; set; }

        /// <summary>
        /// The access token issued by the 10Duke Entitlement service.
        /// </summary>
        public string AccessToken => (string)this["access_token"]?.Value;

        /// <summary>
        /// The type of the issued token.
        /// </summary>
        public string TokenType => (string)this["token_type"]?.Value;

        /// <summary>
        /// The lifetime in seconds of the access token.
        /// </summary>
        public long ExpiresIn => (long)this["expires_in"]?.Value;

        /// <summary>
        /// The refresh token, optional.
        /// </summary>
        public string RefreshToken => (string)this["refresh_token"]?.Value;

        /// <summary>
        /// The scope of the access token, optional.
        /// </summary>
        public string Scope => (string)this["scope"]?.Value;

        /// <summary>
        /// OpenID Connect ID token as a raw string (as returned in the OAuth response), optional.
        /// </summary>
        public string IDTokenRaw => (string)this["id_token"]?.Value;

        /// <summary>
        /// Indicates whether user would prefer this client application to store the access token response
        /// and use the same access token in subsequent client sessions.
        /// </summary>
        public bool? Remember => (bool?)this["remember"]?.Value;

        /// <summary>
        /// RSA public key to use for verifying ID token signature.
        /// </summary>
        public RSA SignerKey { get; set; }

        /// <summary>
        /// Gets <see cref="IDToken"/> object for accessing values of the ID token received from the server.
        /// If signer key is specified, signature of the ID token is verified.
        /// </summary>
        public IDToken IDToken
        {
            get
            {
                if (parsedIdToken == null && IDTokenRaw != null)
                {
                    parsedIdToken = IDToken.Parse(IDTokenRaw, SignerKey);
                }

                return parsedIdToken;
            }
        }

        /// <summary>
        /// Access fields of the access token using the OAuth 2.0 field names / field names
        /// used in the access token response received from the server.
        /// </summary>
        /// <param name="name">The access token field name.</param>
        /// <returns>Value of the response field, or <c>null</c> if no such field found or if response object not populated.</returns>
        public JValue this[string name] => ResponseObject?[name];

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class.
        /// </summary>
        public AccessTokenResponse()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class from serialized representation.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected AccessTokenResponse(SerializationInfo info, StreamingContext context)
        {
            string responseJson = info.GetString("ResponseObject");
            ResponseObject = responseJson == null ? null : JsonConvert.DeserializeObject(responseJson);
            object rsaParameters = info.GetValue("SignerKey", typeof(object));
            if (rsaParameters != null)
            {
                var rsa = new RSACryptoServiceProvider();
                if (rsaParameters is RSAParametersSerializable) {
                    rsa.ImportParameters((rsaParameters as RSAParametersSerializable).RSAParameters);
                }
                else
                {
                    rsa.ImportParameters((rsaParameters as RSAParameters?).Value);
                }
                SignerKey = rsa;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class by the given
        /// access token JSON response.
        /// </summary>
        /// <param name="json">Access token response as a JSON string.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying OpenID Connect ID token signature, if an ID token is present in the response.
        /// If <c>null</c>, no verification is done.</param>
        /// <returns><see cref="AccessTokenResponse"/> object wrapping the access token response received from the server.</returns>
        public static AccessTokenResponse FromJsonStringResponse(string json, RSA verifyWithKey)
        {
            dynamic responseObj = JsonConvert.DeserializeObject(json);
            return FromResponseObject(responseObj, verifyWithKey);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessTokenResponse"/> class by the given
        /// access token response parsed as a dynamic object.
        /// </summary>
        /// <param name="responseObj">Access token JSON response received from the server, parsed as a dynamic object.</param>
        /// <param name="verifyWithKey">RSA public key to use for verifying OpenID Connect ID token signature, if an ID token is present in the response.
        /// If <c>null</c>, no verification is done.</param>
        /// <returns><see cref="AccessTokenResponse"/> object wrapping the access token response received from the server.</returns>
        public static AccessTokenResponse FromResponseObject(dynamic responseObj, RSA verifyWithKey)
        {
            return new AccessTokenResponse()
            {
                ResponseObject = responseObj,
                SignerKey = verifyWithKey
            };
        }

        /// <summary>
        /// Gets data for serialization.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            string responseJson = ResponseObject == null ? null : JsonConvert.SerializeObject(ResponseObject);
            info.AddValue("ResponseObject", responseJson);
            RSAParametersSerializable rsaParameters =
                SignerKey == null
                ? null
                : new RSAParametersSerializable(SignerKey.ExportParameters(false));
            info.AddValue("SignerKey", rsaParameters);
        }

        #endregion
    }
}
