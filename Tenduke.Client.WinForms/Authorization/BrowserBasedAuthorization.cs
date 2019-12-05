﻿using System;
using System.Collections.Specialized;
using System.Web;
using System.Windows.Forms;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;

namespace Tenduke.Client.WinForms.Authorization
{
    /// <summary>
    /// Base class for authorization implementations that use an embedded web browser
    /// for displaying user interface as necessary.
    /// </summary>
    /// <typeparam name="T">Type of the implementing class.</typeparam>
    /// <typeparam name="O">OAuth 2.0 configuration object type.</typeparam>
    /// <typeparam name="A">Authorization process argument type.</typeparam>
    [Serializable]
    public abstract class BrowserBasedAuthorization<T, O, A> : Authorization<T, O, A>
            where T : BrowserBasedAuthorization<T, O, A>
            where O : IBrowserBasedAuthorizationConfig
            where A : BrowserBasedAuthorizationArgs
    {
        #region Private fields

        /// <summary>
        /// Internal field holding WebBrowserForm used for displaying the embedded web browser.
        /// </summary>
        [NonSerialized]
        private WebBrowserForm webBrowserForm;

        #endregion

        #region Properties

        /// <summary>
        /// Form used for displaying the embedded web browser.
        /// </summary>
        protected WebBrowserForm WebBrowserForm
        {
            get
            {
                return webBrowserForm;
            }

            set
            {
                webBrowserForm = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the authorization process and waits for the process to complete before returning.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <returns>Returns OAuth state as received back from the server, or <c>null</c> if OAuth state
        /// is not used with the authorization flow, or if no OAuth state specified in the <paramref name="args"/>.</returns>
        public override string AuthorizeSync(A args)
        {
            OnStarted();

            var authzUri = BuildAuthorizationUri(args);
            string retValue;
            using (var webBrowserForm = InitializeWebBrowserForm(authzUri))
            {
                WebBrowserForm = webBrowserForm;

                OnBeforeAuthorization(args, authzUri);

                var result = webBrowserForm.ShowDialog();
                var cancelled = result != DialogResult.OK;

                if (cancelled)
                {
                    OnCancelled(args, authzUri);
                    retValue = null;
                }
                else
                {
                    var responseParams = ParseResponseParameters(webBrowserForm.ResponseUri);
                    OnAfterAuthorization(args, authzUri, responseParams);

                    retValue = ReadAuthorizationResponse(args, responseParams);
                }

                WebBrowserForm = null;
            }

            return retValue;
        }

        /// <summary>
        /// Creates and initializes a <see cref="WebBrowserForm"/> to use for user interaction
        /// in the authorization process.
        /// </summary>
        /// <param name="initialAddress">The initial address.</param>
        /// <returns>The <see cref="WebBrowserForm"/>.</returns>
        protected virtual WebBrowserForm InitializeWebBrowserForm(Uri initialAddress)
        {
            return new WebBrowserForm()
            {
                Address = initialAddress.ToString(),
                RedirectUri = OAuthConfig.RedirectUri
            };
        }

        /// <summary>
        /// Parses response parameters from the request used by the server for sending the
        /// OAuth response.
        /// </summary>
        /// <param name="responseUri">Request Uri used by the server for redirecting back to the client
        /// for sending the response.</param>
        /// <returns>NameValueCollection containing the parsed response parameters.</returns>
        protected abstract NameValueCollection ParseResponseParameters(string responseUri);

        /// <summary>
        /// Gets the OAuth 2.0 <c>response_type</c> value to use.
        /// </summary>
        /// <returns>The response type value.</returns>
        protected abstract string GetResponseType();

        /// <summary>
        /// Builds the full Uri for starting the authorization process on the server.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <returns>Uri to use as the initial Uri where the embedded browser is opened.</returns>
        protected Uri BuildAuthorizationUri(A args)
        {
            if (OAuthConfig == null)
            {
                throw new InvalidOperationException("OAuthConfig must be specified");
            }

            if (OAuthConfig.ClientID == null)
            {
                throw new InvalidOperationException("OAuthConfig.ClientID must be specified");
            }

            if (OAuthConfig.AuthzUri == null)
            {
                throw new InvalidOperationException("OAuthConfig.AuthzUri must be specified");
            }

            var uriBuilder = new UriBuilder(OAuthConfig.AuthzUri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["response_type"] = GetResponseType();
            query["client_id"] = OAuthConfig.ClientID;
            query["showRememberMe"] = OAuthConfig.ShowRememberMe ? "true" : "false";

            if (OAuthConfig.RedirectUri != null)
            {
                query["redirect_uri"] = OAuthConfig.RedirectUri;
            }

            if (OAuthConfig.Scope != null)
            {
                query["scope"] = OAuthConfig.Scope;
            }

            if (OAuthConfig.Scope != null)
            {
                query["scope"] = OAuthConfig.Scope;
            }

            if (args != null && args.State != null)
            {
                query["state"] = args.State;
            }

            if (args != null && args.Nonce != null)
            {
                query["nonce"] = args.Nonce;
            }

            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        #endregion
    }
}
