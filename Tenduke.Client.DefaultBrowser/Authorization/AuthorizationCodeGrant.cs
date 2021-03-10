using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.DefaultBrowser.Config;
using Tenduke.Client.Util;

namespace Tenduke.Client.DefaultBrowser.Authorization
{
    /// <summary>
    /// Authorization Code Grant flow using OS default browser.
    /// </summary>
    public class AuthorizationCodeGrant : Authorization<
        AuthorizationCodeGrant, IDefaultBrowserAuthorizationCodeGrantConfig, AuthorizationCodeGrantArgs>
    {
        #region Public constants

        /// <summary>
        /// Default timeout for waiting for response of the authentication / authorization process, in seconds.
        /// </summary>
        public static readonly int DEFAULT_RESPONSE_TIMEOUT_SECONDS = 300;

        #endregion

        #region Properties

        /// <summary>
        /// Resolver for HTML response to send after OIDC authentication / authorization
        /// process has been completed. If not specified, default HTML response is returned.
        /// </summary>
        public IAuthorizationCompletedResponseResolver AuthorizationCompletedResponseResolver { get; set; }

        /// <summary>
        /// The authorization code received from the 10Duke Entitlement service.
        /// </summary>
        protected string AuthorizationCode { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the authorization process and waits for the process to complete before returning.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <returns>Returns OAuth state as received back from the server, or <c>null</c> if OAuth state
        /// is not used with the authorization flow, or if no OAuth state specified in the <paramref name="args"/>.</returns>
        public override string AuthorizeSync(AuthorizationCodeGrantArgs args)
        {
            var authorizeTask = Authorize(args, null);
            authorizeTask.Wait();
            return authorizeTask.Result;
        }

        /// <summary>
        /// Starts the authorization process.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="cancellationToken">Cancellation token for cancelling the operation.</param>
        /// <returns>Returns OAuth state as received back from the server, or <c>null</c> if OAuth state
        /// is not used with the authorization flow, or if no OAuth state specified in the <paramref name="args"/>.</returns>
        public async Task<string> Authorize(AuthorizationCodeGrantArgs args, CancellationToken? cancellationToken)
        {
            OnStarted();

            HttpListener httpListener;
            int port;
            string redirectUri;
            var result = TryBindListenerOnFreePort(out httpListener, out port, out redirectUri);
            if (!result)
            {
                throw new InvalidOperationException(
                    "Opening port for listening to authentication response failed. " +
                    "Please check settings of your security software and ensure that this " +
                    "application is allowed to open an HTTP listener in a free port between 49215 - 65535.");
            }

            try
            {
                var configWithDynamicRedirectUri = new AuthorizationCodeGrantConfigWithDynamicRedirectUri(OAuthConfig, redirectUri);
                var authzUri = AuthorizationUri.BuildAuthorizationUri(configWithDynamicRedirectUri, GetResponseType(), args);

                OnBeforeAuthorization(args, authzUri);

                // Open URL in the OS default browser
                Process.Start(new ProcessStartInfo(authzUri.ToString()) { UseShellExecute = true });

                // Wait for the response (request sent to the redirectUri)
                var timeoutSeconds = GetResponseTimeout();
                var waitForResponseTask = httpListener.GetContextAsync();
                var timeoutTask = cancellationToken == null
                        ? Task.Delay(timeoutSeconds * 1000)
                        : Task.Delay(timeoutSeconds * 1000, cancellationToken.Value);
                if (await Task.WhenAny(waitForResponseTask, timeoutTask).ConfigureAwait(false) == waitForResponseTask)
                {
                    await waitForResponseTask;
                }
                else
                {
                    OnCancelled(args, authzUri);
                    return null;
                }

                var context = waitForResponseTask.Result;
                var responseParams = ParseResponseParameters(context.Request.Url);
                OnAfterAuthorization(args, authzUri, responseParams);

                SendAuthorizationCompletedResponse(args, authzUri, responseParams, context);

                return ReadAuthorizationResponse(configWithDynamicRedirectUri, args, responseParams);
            }
            finally
            {
                httpListener.Close();
            }
        }

        /// <summary>
        /// Sends response to the browser after receiving the OIDC authentication / authorization response.
        /// This OIDC response is received as a redirect request, and parameters of the request contain the
        /// response.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="authzUri">The full Uri used for calling the authorization endpoint.</param>
        /// <param name="responseParameters">Response parameters received in the response from the server.</param>
        /// <param name="httpContext"><see cref="HttpListenerContext"/> for the listener that received the OIDC response
        /// and that is used for writing back the browser response.</param>
        protected void SendAuthorizationCompletedResponse(
            AuthorizationCodeGrantArgs args,
            Uri authzUri,
            NameValueCollection responseParameters,
            HttpListenerContext httpContext)
        {
            using (var responseReader =
                AuthorizationCompletedResponseResolver?.GetAuthorizationCompletedHtml(args, authzUri, responseParameters)
                ?? GetDefaultAuthorizationCompletedHtmlReader())
            {
                using (var response = httpContext.Response)
                {
                    response.ContentType = "text/html";
                    response.ContentEncoding = Encoding.UTF8;
                    using (var responseOutput = new StreamWriter(response.OutputStream))
                    {
                        while (true)
                        {
                            string line = responseReader.ReadLine();
                            if (line == null)
                            {
                                break;
                            }
                            responseOutput.WriteLine(line);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets reader for reading the default HTML file for sending browser response after received
        /// the OIDC response.
        /// </summary>
        /// <returns>Reader for reading the HTML content. Caller of this method is responsible for closing the reader.</returns>
        protected TextReader GetDefaultAuthorizationCompletedHtmlReader()
        {
            return new StreamReader(GetDefaultAuthorizationCompletedHtmlStream());
        }

        /// <summary>
        /// Parses response parameters from the request used by the server for sending the
        /// OAuth response.
        /// </summary>
        /// <param name="responseUri">Request Uri used by the server for redirecting back to the client
        /// for sending the response.</param>
        /// <returns>NameValueCollection containing the parsed response parameters.</returns>
        protected NameValueCollection ParseResponseParameters(Uri responseUri)
        {
            return HttpUtility.ParseQueryString(responseUri.Query);
        }

        /// <summary>
        /// Reads response and populates this object from the response parameters received
        /// from the server as a response to an authorization request.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="responseParameters">The response parameters from the server.</param>
        /// <returns>Returns the OAuth state read from the response parameters, or <c>null</c> if no <c>state</c>
        /// response parameter found.</returns>
        protected override string ReadAuthorizationResponse(
            AuthorizationCodeGrantArgs args,
            NameValueCollection responseParameters)
        {
            throw new InvalidOperationException(
                "Must use dynamic redirect URI configuration with the OS default browser based client");
        }

        /// <summary>
        /// Reads response and populates this object from the response parameters received
        /// from the server as a response to an authorization request.
        /// </summary>
        /// <param name="config">Client configuration, overridden to use the contain the dynamic
        /// redirect URI.</param>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="responseParameters">The response parameters from the server.</param>
        /// <returns>Returns the OAuth state read from the response parameters, or <c>null</c> if no <c>state</c>
        /// response parameter found.</returns>
        protected string ReadAuthorizationResponse(
            IAuthorizationCodeGrantConfig config,
            AuthorizationCodeGrantArgs args,
            NameValueCollection responseParameters)
        {
            AuthorizationCode = responseParameters["code"];
            var retValue = base.ReadAuthorizationResponse(args, responseParameters);

            if (AuthorizationCode != null)
            {
                try
                {
                    // Authorization code has been received, call the token endpoint for getting an access token
                    RequestAndHandleAccessToken(config, args);
                }
                finally
                {
                    AuthorizationCode = null;
                }
            }

            return retValue;
        }

        /// <summary>
        /// Gets timeout for waiting for response of the authentication / authorization process.
        /// </summary>
        /// <returns>Timeout in seconds</returns>
        protected int GetResponseTimeout()
        {
            if (OAuthConfig.ResponseTimeout != null)
            {
                return OAuthConfig.ResponseTimeout.Value;
            }

            return DEFAULT_RESPONSE_TIMEOUT_SECONDS;
        }

        /// <summary>
        /// Gets the OAuth 2.0 <c>response_type</c> value to use.
        /// </summary>
        /// <returns>The response type value.</returns>
        protected string GetResponseType()
        {
            return OAuthUtil.RESPONSE_TYPE_CODE;
        }

        /// <summary>
        /// Opens a <see cref="HttpListener"/> in an available port.
        /// <see cref="https://stackoverflow.com/a/46666370"/>
        /// </summary>
        /// <param name="httpListener">The started <see cref="HttpListener"/>. Caller of this method is
        /// responsible for closing the listener.</param>
        /// <param name="port">The opened port</param>
        /// <param name="listenerUri">URI for accessing the opened listener</param>
        /// <returns><c>true</c> if opened successfully <c>false</c> otherwise</returns>
        protected bool TryBindListenerOnFreePort(out HttpListener httpListener, out int port, out string listenerUri)
        {
            // IANA suggested range for dynamic or private ports
            const int MinPort = 49215;
            const int MaxPort = 65535;

            for (port = MinPort; port < MaxPort; port++)
            {
                httpListener = new HttpListener();
                listenerUri = string.Format("http://{0}:{1}/", IPAddress.Loopback, port);
                httpListener.Prefixes.Add(listenerUri);
                try
                {
                    httpListener.Start();
                    return true;
                }
                catch
                {
                    // nothing to do here -- the listener disposes itself when Start throws
                }
            }

            port = 0;
            httpListener = null;
            listenerUri = null;
            return false;
        }

        /// <summary>
        /// Sends a request for exchanging authorization code to an access token, and handles the response.
        /// </summary>
        /// <param name="config">Client configuration, overridden to use the contain the dynamic
        /// redirect URI.</param>
        /// <param name="args">Authorization operation arguments.</param>
        protected void RequestAndHandleAccessToken(
            IAuthorizationCodeGrantConfig config,
            AuthorizationCodeGrantArgs args)
        {
            string jsonResponse = RequestAccessToken(config, args);
            ReadAccessTokenResponse(jsonResponse);
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
            ReadAccessTokenResponse(jsonResponse);
        }

        /// <summary>
        /// Sends a request for exchanging authorization code to an access token.
        /// </summary>
        /// <param name="config">Client configuration, overridden to use the contain the dynamic
        /// redirect URI.</param>
        /// <param name="args">Authorization operation arguments.</param>
        protected string RequestAccessToken(
            IAuthorizationCodeGrantConfig config,
            AuthorizationCodeGrantArgs args)
        {
            return OAuthUtil.RequestAccessToken(AuthorizationCode, config, args.CodeVerifier);
        }

        /// <summary>
        /// Parses response from the server to an access token request, and populates fields of this object.
        /// </summary>
        /// <param name="accessTokenResponse">Dynamic object representing the JSON response received from the server.</param>
        protected override void ReadAccessTokenResponse(dynamic accessTokenResponse)
        {
            ReadAccessTokenResponseCommon(accessTokenResponse);
            AccessTokenResponse =
                accessTokenResponse["access_token"] == null
                ? null
                : Client.Authorization.AccessTokenResponse.FromResponseObject(accessTokenResponse, OAuthConfig.SignerKey);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets stream for reading the default HTML file for sending browser response after received
        /// the OIDC response.
        /// </summary>
        /// <returns>Stream for reading the file. Caller of this method is responsible for closing the stream.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        private Stream GetDefaultAuthorizationCompletedHtmlStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Tenduke.Client.DefaultBrowser.resources.authCompleted.html");
        }

        #endregion

        #region Nested BrowserBasedAuthorizationConfigWithDynamicRedirectUri

        private class AuthorizationCodeGrantConfigWithDynamicRedirectUri : AuthorizationCodeGrantConfigWrapper
        {
            private string redirectUri;

            public override string RedirectUri
            {
                get
                {
                    return redirectUri;
                }
            }

            public AuthorizationCodeGrantConfigWithDynamicRedirectUri(IAuthorizationCodeGrantConfig wrapped, string redirectUri)
                : base(wrapped)
            {
                this.redirectUri = redirectUri;
            }
        }

        #endregion
    }
}
