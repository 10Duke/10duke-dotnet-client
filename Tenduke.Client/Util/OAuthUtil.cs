using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using Tenduke.Client.Config;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// OAuth 2.0 utilities.
    /// </summary>
    public static class OAuthUtil
    {
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

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true,
                AllowAutoRedirect = false
            };

            var pairs = new List<KeyValuePair<string, string>>()
            {
                new("grant_type", HttpUtility.UrlEncode(GRANT_TYPE_AUTHORIZATION_CODE)),
                new("code", HttpUtility.UrlEncode(code)),
                new("client_id", HttpUtility.UrlEncode(oauthConfig.ClientID)),
            };

            if (oauthConfig.RedirectUri != null)
            {
                pairs.Add(new("redirect_uri", oauthConfig.RedirectUri));
            }

            if (oauthConfig.ClientSecret != null && !oauthConfig.UsePkce)
            {
                pairs.Add(new("client_secret", oauthConfig.ClientSecret));
            }

            if (codeVerifier != null && oauthConfig.UsePkce)
            {
                pairs.Add(new("code_verifier", codeVerifier));
            }

            using var httpClient = new HttpClient(handler);
            using var res = httpClient.PostAsync(oauthConfig.TokenUri, new FormUrlEncodedContent(pairs));

            string jsonResponse = res.Result.Content.ReadAsStringAsync().Result;

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

            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) => true,
                AllowAutoRedirect = false
            };

            var pairs = new List<KeyValuePair<string, string>>()
            {
                new("grant_type", GRANT_TYPE_REFRESH_TOKEN),
                new("refresh_token", refreshToken),
                new("client_id", oauthConfig.ClientID),
            };

            if (!oauthConfig.UsePkce && !string.IsNullOrEmpty(oauthConfig.ClientSecret))
            {
                pairs.Add(new("client_secret", oauthConfig.ClientSecret));
            }

            if (scope != null)
            {
                pairs.Add(new("scope", scope));
            }

            using var httpClient = new HttpClient(handler);
            using var res = httpClient.PostAsync(oauthConfig.TokenUri, new FormUrlEncodedContent(pairs));

            string jsonResponse = res.Result.Content.ReadAsStringAsync().Result;

            return jsonResponse;
        }
    }
}
