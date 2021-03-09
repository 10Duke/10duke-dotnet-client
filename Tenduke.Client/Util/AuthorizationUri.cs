using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// Utility for building URI to use when calling the OAuth / OpenID Connect
    /// authorization / authentication endpoint.
    /// </summary>
    public static class AuthorizationUri
    {
        #region Private fields

        /// <summary>
        /// Function that builds authorization URI query part.
        /// </summary>
        /// <typeparam name="A">Type of argument object</typeparam>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        /// <param name="responseType">OIDC response type</param>
        /// <param name="args">Arguments for the OIDC process to start</param>
        /// <param name="query">Query parameters to populate</param>
        private delegate void AuthorizationUriQueryBuilderFunc<A>(
            IBrowserBasedAuthorizationConfig config,
            string responseType,
            A args,
            NameValueCollection query) where A : BrowserBasedAuthorizationArgs;

        #endregion

        #region Methods

        /// <summary>
        /// Builds the full Uri for starting the OpenID Connect authentication / authorization process against the server
        /// using browser-based Authorization Code Grant flow (optionally with PKCE).
        /// </summary>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        /// <param name="responseType">OIDC response type</param>
        /// <param name="args"><see cref="AuthorizationCodeGrantArgs"/> specifying arguments for the process to start</param>
        /// <returns><see cref="Uri"/> to use for starting the process</returns>
        public static Uri BuildAuthorizationUri(IBrowserBasedAuthorizationConfig config, string responseType, AuthorizationCodeGrantArgs args)
        {
            return BuildAuthorizationUriInternal(config, responseType, args, BuildAuthorizationUriQuery);
        }

        /// <summary>
        /// Builds the full Uri for starting the OpenID Connect authentication / authorization process against the server
        /// using browser-based authentication / authorization.
        /// </summary>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        /// <param name="responseType">OIDC response type</param>
        /// <param name="args"><see cref="BrowserBasedAuthorizationArgs"/> specifying arguments for the process to start</param>
        /// <returns><see cref="Uri"/> to use for starting the process</returns>
        public static Uri BuildAuthorizationUri(IBrowserBasedAuthorizationConfig config, string responseType, BrowserBasedAuthorizationArgs args)
        {
            return BuildAuthorizationUriInternal(config, responseType, args, BuildAuthorizationUriQuery);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Builds the full Uri for starting the OpenID Connect authentication / authorization process against the server
        /// using browser-based authentication / authorization.
        /// </summary>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        /// <param name="responseType">OIDC response type</param>
        /// <param name="args">Arguments for the OIDC process to start</param>
        /// <returns><see cref="Uri"/> to use for starting the process</returns>
        private static Uri BuildAuthorizationUriInternal<A>(
            IBrowserBasedAuthorizationConfig config,
            string responseType,
            A args,
            AuthorizationUriQueryBuilderFunc<A> authzUriQueryBuilder) where A : BrowserBasedAuthorizationArgs
        {
            AssertConfig(config);

            var uriBuilder = new UriBuilder(config.AuthzUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            authzUriQueryBuilder(config, responseType, args, query);
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        /// <summary>
        /// Asserts that the given OAuth / OIDC configuration is valid.
        /// </summary>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        private static void AssertConfig(IBrowserBasedAuthorizationConfig config)
        {
            if (config == null)
            {
                throw new InvalidOperationException("OAuthConfig must be specified");
            }

            if (config.ClientID == null)
            {
                throw new InvalidOperationException("OAuthConfig.ClientID must be specified");
            }

            if (config.AuthzUri == null)
            {
                throw new InvalidOperationException("OAuthConfig.AuthzUri must be specified");
            }
        }

        /// <summary>
        /// Builds the full Uri for starting the OpenID Connect authentication / authorization process against the server
        /// using browser-based Authorization Code Grant flow (optionally with PKCE).
        /// </summary>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        /// <param name="responseType">OIDC response type</param>
        /// <param name="args"><see cref="AuthorizationCodeGrantArgs"/> specifying arguments for the process to start</param>
        /// <param name="query">Query parameters to populate</param>
        /// <returns><see cref="Uri"/> to use for starting the process</returns>
        private static void BuildAuthorizationUriQuery(
            IBrowserBasedAuthorizationConfig config,
            string responseType,
            AuthorizationCodeGrantArgs args,
            NameValueCollection query)
        {
            BuildAuthorizationUriQuery(config, responseType, args as BrowserBasedAuthorizationArgs, query);

            if (args != null && args.CodeVerifier != null)
            {
                query["code_challenge_method"] = "S256";
                query["code_challenge"] = args.ComputeCodeChallenge();
            }
        }

        /// <summary>
        /// Builds the full Uri for starting the OpenID Connect authentication / authorization process against the server
        /// using browser-based authentication / authorization.
        /// </summary>
        /// <param name="config">OAuth / OpenID Connect configuration of the client</param>
        /// <param name="responseType">OIDC response type</param>
        /// <param name="args"><see cref="BrowserBasedAuthorizationArgs"/> specifying arguments for the process to start</param>
        /// <param name="query">Query parameters to populate</param>
        /// <returns><see cref="Uri"/> to use for starting the process</returns>
        private static void BuildAuthorizationUriQuery(
            IBrowserBasedAuthorizationConfig config,
            string responseType,
            BrowserBasedAuthorizationArgs args,
            NameValueCollection query)
        {
            query["response_type"] = responseType;
            query["client_id"] = config.ClientID;
            query["showRememberMe"] = config.ShowRememberMe ? "true" : "false";

            if (config.RedirectUri != null)
            {
                query["redirect_uri"] = config.RedirectUri;
            }

            if (config.Scope != null)
            {
                query["scope"] = config.Scope;
            }

            if (args != null && args.State != null)
            {
                query["state"] = args.State;
            }

            if (args != null && args.Nonce != null)
            {
                query["nonce"] = args.Nonce;
            }
        }

        #endregion
    }
}
