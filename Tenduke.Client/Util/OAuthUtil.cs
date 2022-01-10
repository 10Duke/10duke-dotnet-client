using System;
using System.IO;
using System.Net;
using System.Web;
using Tenduke.Client.Config;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// OAuth 2.0 utilities.
    /// </summary>
    public static class OAuthUtil
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

        /// <summary>
        /// OAuth 2.0 <c>grant_type</c> value <c>refresh_token</c>.
        /// </summary>
        public static readonly string GRANT_TYPE_REFRESH_TOKEN = "refresh_token";

        #endregion

        #region Methods

        /// <summary>
        /// Exchanges an authorization code received earlier during OAuth 2.0 Authorization Code Grant flow execution
        /// to an access token.
        /// </summary>
        /// <param name="code">The authorization code.</param>
        /// <param name="oauthConfig">OAuth configuration.</param>
        /// <param name="codeVerifier">PKCE (Proog Key for Code Exchange) code verifier,
        /// if using PKCE to secure the Authorization Code Grant flow.</param>
        /// <returns>The access token response as a string.</returns>
        public static string RequestAccessToken(string code, IAuthorizationCodeGrantConfig oauthConfig, string codeVerifier = null)
        {
            if (oauthConfig.TokenUri == null)
            {
                throw new InvalidOperationException("OAuthConfig.TokenUri must be specified");
            }

            var tokenRequest = WebRequest.CreateHttp(oauthConfig.TokenUri);
            if (oauthConfig.AllowInsecureCerts)
            {
                tokenRequest.ServerCertificateValidationCallback =
                    (sender, certificate, chain, sslPolicyErrors) => true;
            }
            tokenRequest.Method = "POST";
            tokenRequest.AllowAutoRedirect = false;
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            using (var requestStreamWriter = new StreamWriter(tokenRequest.GetRequestStream()))
            {
                requestStreamWriter.Write("grant_type=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(GRANT_TYPE_AUTHORIZATION_CODE));
                requestStreamWriter.Write("&code=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(code));
                requestStreamWriter.Write("&client_id=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(oauthConfig.ClientID));

                if (oauthConfig.RedirectUri != null)
                {
                    requestStreamWriter.Write("&redirect_uri=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(oauthConfig.RedirectUri));
                }

                if (oauthConfig.ClientSecret != null && !oauthConfig.UsePkce)
                {
                    requestStreamWriter.Write("&client_secret=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(oauthConfig.ClientSecret));
                }

                if (codeVerifier != null && oauthConfig.UsePkce)
                {
                    requestStreamWriter.Write("&code_verifier=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(codeVerifier));
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
        /// Exchanges a refresh token received earlier during OAuth 2.0 Authorization Code Grant flow execution
        /// to a new access token.
        /// </summary>
        /// <param name="refreshToken">The refresh token.</param>
        /// <param name="oauthConfig">OAuth configuration.</param>
        /// <param name="scope">OAuth scope, optional. If <c>null</c>, requesting the same scope that had been granted
        /// with the original access token.</param>
        /// <returns>The access token response as a string.</returns>
        public static string RefreshAccessToken(string refreshToken, IAuthorizationCodeGrantConfig oauthConfig, string scope = null)
        {
            if (oauthConfig.TokenUri == null)
            {
                throw new InvalidOperationException("OAuthConfig.TokenUri must be specified");
            }

            var tokenRequest = WebRequest.CreateHttp(oauthConfig.TokenUri);
            tokenRequest.Method = "POST";
            tokenRequest.AllowAutoRedirect = false;
            tokenRequest.ContentType = "application/x-www-form-urlencoded";
            using (var requestStreamWriter = new StreamWriter(tokenRequest.GetRequestStream()))
            {
                requestStreamWriter.Write("grant_type=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(GRANT_TYPE_REFRESH_TOKEN));
                requestStreamWriter.Write("&refresh_token=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(refreshToken));
                requestStreamWriter.Write("&client_id=");
                requestStreamWriter.Write(HttpUtility.UrlEncode(oauthConfig.ClientID));

                if (oauthConfig.ClientSecret != null)
                {
                    requestStreamWriter.Write("&client_secret=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(oauthConfig.ClientSecret));
                }

                if (scope != null)
                {
                    requestStreamWriter.Write("&scope=");
                    requestStreamWriter.Write(HttpUtility.UrlEncode(scope));
                }
            }

            string jsonResponse;
            try
            {
                using (var response = tokenRequest.GetResponse())
                using (var responseStreamReader = new StreamReader(response.GetResponseStream()))
                {
                    jsonResponse = responseStreamReader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                using (var errorResponse = ex.Response.GetResponseStream())
                using (var errorResponseStreamReader = new StreamReader(errorResponse))
                {
                    jsonResponse = errorResponseStreamReader.ReadToEnd();
                }
            }

            return jsonResponse;
        }

        #endregion
    }
}
