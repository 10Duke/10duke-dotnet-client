using System;
using System.Collections.Specialized;
using System.Windows.Forms;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.Util;

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

        #region Events

        /// <summary>
        /// Raised when initializing the web browser form. Subscribers of the event may
        /// set properties of the form.
        /// </summary>
        public event EventHandler<InitializeBrowserFormEventArgs> RaiseInitializeBrowserForm;

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

        /// <summary>
        /// Indicates if CEF console logging is enabled.
        /// </summary>
        public bool EnableCefConsoleLogging { get; set; }

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
            var webBrowserForm = new WebBrowserForm()
            {
                Address = initialAddress.ToString(),
                RedirectUri = OAuthConfig.RedirectUri,
                AllowInsecureCerts = OAuthConfig.AllowInsecureCerts,
                EnableCefConsoleLogging = EnableCefConsoleLogging,
            };
            OnRaiseInitializeBrowserForm(new InitializeBrowserFormEventArgs(webBrowserForm));
            return webBrowserForm;
        }

        /// <summary>
        /// Called for invoking the <see cref="RaiseInitializeBrowserForm"/> event.
        /// </summary>
        /// <param name="e">The <see cref="InitializeBrowserFormEventArgs"/>.</param>
        protected virtual void OnRaiseInitializeBrowserForm(InitializeBrowserFormEventArgs e)
        {
            EventHandler<InitializeBrowserFormEventArgs> handler = RaiseInitializeBrowserForm;
            if (handler != null)
            {
                handler(this, e);
            }
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
        protected virtual Uri BuildAuthorizationUri(A args)
        {
            return AuthorizationUri.BuildAuthorizationUri(OAuthConfig, GetResponseType(), args);
        }

        #endregion
    }
}
