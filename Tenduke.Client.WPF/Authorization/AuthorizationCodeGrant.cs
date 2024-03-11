using System;
using System.Collections.Specialized;
using System.Web;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.Util;

namespace Tenduke.Client.WPF.Authorization
{
    /// <summary>
    /// Authorizes with the OAuth 2.0 Authorization Code Grant flow, using an embedded browser
    /// for displaying user interface.
    /// </summary>
    [Serializable]
    public class AuthorizationCodeGrant
        : BrowserBasedAuthorization<IAuthorizationCodeGrantConfig, AuthorizationCodeGrantArgs>
    {
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
            return OAuthUtil.RESPONSE_TYPE_CODE;
        }

        /// <summary>
        /// Sends a request for exchanging authorization code to an access token, and handles the response.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        protected void RequestAndHandleAccessToken(AuthorizationCodeGrantArgs args)
        {
            string jsonResponse = RequestAccessToken(args);
            ReadAccessTokenResponse(jsonResponse, OAuthConfig.SignerKey);
        }

        /// <summary>
        /// Sends a request for exchanging refresh token received earlier with an access token response
        /// to a new access token, and handles the response.
        /// </summary>
        public void RefreshAuthorization()
        {
            if (AccessTokenResponse == null)
            {
                throw new InvalidOperationException("There is no current authorization based on an OAuth access token response, can not refresh authorization");
            }

            if (AccessTokenResponse.RefreshToken == null)
            {
                throw new InvalidOperationException("There server has not issued a refresh token, can not refresh authorization");
            }

            string jsonResponse = OAuthUtil.RefreshAccessToken(AccessTokenResponse.RefreshToken, OAuthConfig);
            ReadAccessTokenResponse(jsonResponse, OAuthConfig.SignerKey);
        }

        /// <summary>
        /// Sends a request for exchanging authorization code to an access token.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        protected string RequestAccessToken(AuthorizationCodeGrantArgs args)
        {
            return OAuthUtil.RequestAccessToken(AuthorizationCode, OAuthConfig, args.CodeVerifier);
        }

        /// <summary>
        /// Builds the full Uri for starting the authorization process on the server.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <returns>Uri to use as the initial Uri where the embedded browser is opened.</returns>
        protected override Uri BuildAuthorizationUri(AuthorizationCodeGrantArgs args)
        {
            return AuthorizationUri.BuildAuthorizationUri(OAuthConfig, GetResponseType(), args);
        }

        #endregion
    }
}
