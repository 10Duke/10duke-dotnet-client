using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;

namespace Tenduke.Client.WPF.Authorization
{
    /// <summary>
    /// Authorizes with the OAuth 2.0 Authorization Code Grant flow, using an embedded browser
    /// for displaying user interface.
    /// </summary>
    [Serializable]
    public class AuthorizationCodeGrant
        : BrowserBasedAuthorization<AuthorizationCodeGrant, IAuthorizationCodeGrantConfig, AuthorizationCodeGrantArgs>
    {
        #region Public constants

        /// <summary>
        /// OAuth 2.0 <c>response_type</c> value <c>code</c>, as used with the Authorization Code Grant flow.
        /// </summary>
        public static readonly string RESPONSE_TYPE_CODE = "code";

        /// <summary>
        /// OAuth 2.0 <c>grant_type</c> value <c>authorization_code</c>.
        /// </summary>
        public static readonly string GRANT_TYPE_AUTHORIZATION_CODE = "authorization_code";

        #endregion

        #region Properties

        /// <summary>
        /// The authorization code received from the 10Duke Entitlement service.
        /// </summary>
        protected string AuthorizationCode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Reads response and populates this object from the response parameters received
        /// from the server as a response to an authorization request.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="responseParameters">The response parameters from the server.</param>
        /// <returns>Returns the OAuth state read from the response parameters, or <c>null</c> if no <c>state</c>
        /// response parameter found.</returns>
        protected override string ReadAuthorizationResponse(AuthorizationCodeGrantArgs args, NameValueCollection responseParameters)
        {
            AuthorizationCode = responseParameters["code"];
            var retValue = base.ReadAuthorizationResponse(args, responseParameters);

            if (AuthorizationCode != null)
            {
                try
                {
                    // Authorization code has been received, call the token endpoint for getting an access token
                    RequestAndHandleAccessToken(args);
                }
                finally
                {
                    AuthorizationCode = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Parses response parameters from the request used by the server for sending the
        /// OAuth response.
        /// </summary>
        /// <param name="responseUri">Request Uri used by the server for redirecting back to the client
        /// for sending the response.</param>
        /// <returns>NameValueCollection containing the parsed response parameters.</returns>
        protected override NameValueCollection ParseResponseParameters(string responseUri)
        {
            var responseUriParams = HttpUtility.ParseQueryString(new Uri(responseUri).Query);

            // Handle only parameters that are not specified in the configured RedirectUri as
            // server response parameters
            var redirectUriParams = HttpUtility.ParseQueryString(new Uri(OAuthConfig.RedirectUri).Query);
            foreach (var redirectUriParam in redirectUriParams.AllKeys)
            {
                responseUriParams.Remove(redirectUriParam);
            }

            return responseUriParams;
        }

        /// <summary>
        /// Gets the OAuth 2.0 <c>response_type</c> value to use.
        /// </summary>
        /// <returns>The response type value.</returns>
        protected override string GetResponseType()
        {
            return RESPONSE_TYPE_CODE;
        }

        /// <summary>
        /// Sends a request for exchanging authorization code to an access token, and handles the response.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        protected void RequestAndHandleAccessToken(AuthorizationCodeGrantArgs args)
        {
            string jsonResponse = RequestAccessToken(args);
            ReadAccessTokenResponse(jsonResponse);
        }

        /// <summary>
        /// Sends a request for exchanging authorization code to an access token.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        protected string RequestAccessToken(AuthorizationCodeGrantArgs args)
        {
            if (OAuthConfig.TokenUri == null)
            {
                throw new InvalidOperationException("OAuthConfig.TokenUri must be specified");
            }

            var tokenRequest = WebRequest.CreateHttp(OAuthConfig.TokenUri);
            tokenRequest.Method = "POST";
            tokenRequest.AllowAutoRedirect = false;
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            using (var requestStreamWriter = new StreamWriter(tokenRequest.GetRequestStream()))
            {
                requestStreamWriter.Write("grant_type=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(GRANT_TYPE_AUTHORIZATION_CODE));
                requestStreamWriter.Write("&code=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(AuthorizationCode));
                requestStreamWriter.Write("&client_id=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(OAuthConfig.ClientID));

                if (OAuthConfig.RedirectUri != null)
                {
                    requestStreamWriter.Write("&redirect_uri=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(OAuthConfig.RedirectUri));
                }

                if (OAuthConfig.ClientSecret != null)
                {
                    requestStreamWriter.Write("&client_secret=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(OAuthConfig.ClientSecret));
                }
            }

            string jsonResponse;
            using (var response = tokenRequest.GetResponse())
            {
                using (var responseStreamReader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = responseStreamReader.ReadToEnd();
                }
            }

            return jsonResponse;
        }

        /// <summary>
        /// Parses response from the server to an access token request, and populates fields of this object.
        /// </summary>
        /// <param name="accessTokenResponse">JSON string response received from the server.</param>
        protected void ReadAccessTokenResponse(string accessTokenResponse)
        {
            dynamic json = JsonConvert.DeserializeObject(accessTokenResponse);
            Error = json["error"];
            ErrorDescription = json["error_description"];
            ErrorUri = json["error_uri"];
            AccessTokenResponse =
                json["access_token"] == null
                ? null
                : Client.Authorization.AccessTokenResponse.FromResponseObject(json, OAuthConfig.SignerKey);
        }

        #endregion
    }
}
