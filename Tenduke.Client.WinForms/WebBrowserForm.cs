using CefSharp;
using CefSharp.WinForms;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using Tenduke.Client.Desktop.Util;

namespace Tenduke.Client.WinForms
{
    /// <summary>
    /// Form for displaying the embedded browser.
    /// </summary>
    public partial class WebBrowserForm : Form
    {
        #region Private fields

        /// <summary>
        /// Path to file used as loader / in-progress indicator before the initial page is loaded in the browser.
        /// </summary>
        private string loaderPath;

        /// <summary>
        /// Indicates if loading the initial browser page has been started.
        /// </summary>
        private bool initialPageLoadStarted;

        #endregion

        #region Properties

        /// <summary>
        /// The <see cref="ChromiumWebBrowser"/>.
        /// </summary>
        public ChromiumWebBrowser ChromiumWebBrowser { get; set; }

        /// <summary>
        /// The initial address for the browser.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Redirect Uri that the server will use for sending response. Requests to addresses
        /// starting with this Uri are intercepted by this component and interpreted
        /// as server responses.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Full response Uri of the request received from the server, used by the server for sending the response.
        /// </summary>
        public string ResponseUri { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the WebBrowserForm class.
        /// </summary>
        public WebBrowserForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Called when the web browser form is shown.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void WebBrowserForm_Shown(object sender, EventArgs e)
        {
            if (loaderPath == null)
            {
                loaderPath = LoaderFileUtil.WriteLoaderHtmlToTempFile();
            }
            ChromiumWebBrowser = new ChromiumWebBrowser(loaderPath);
            initialPageLoadStarted = false;
            ChromiumWebBrowser.LoadingStateChanged += ChromiumWebBrowser_LoadingStateChanged;
            panelWebBrowserContainer.Controls.Add(ChromiumWebBrowser);
            ChromiumWebBrowser.Dock = DockStyle.Fill;
            ChromiumWebBrowser.RequestHandler = new AuthzRequestHandler(this);
        }

        private void ChromiumWebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!initialPageLoadStarted && !e.IsLoading)
            {
                initialPageLoadStarted = true;
                ChromiumWebBrowser.Load(Address);
            }
        }

        private void WebBrowserForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Controls.Remove(ChromiumWebBrowser);
            ChromiumWebBrowser.Dispose();
            ChromiumWebBrowser = null;
            if (loaderPath != null)
            {
                File.Delete(loaderPath);
                loaderPath = null;
            }
        }

        /// <summary>
        /// Called from <see cref="IRequestHandler.OnProtocolExecution(IWebBrowser, IBrowser, string)"/> of the browser request handler.
        /// Allows this component to intercept OAuth response callbacks.
        /// </summary>
        /// <param name="browserControl">The browser control.</param>
        /// <param name="browser">The browser.</param>
        /// <param name="url">The redirect URL used for sending the OAuth response.</param>
        /// <returns>Always returns <c>false</c>, never letting the browser continue handling after this handler.</returns>
        private bool HandleBrowserProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            if (url.StartsWith(RedirectUri))
            {
                ResponseUri = url;
                Invoke(new Action(() =>
                {
                    // DialogResult.OK indicates that response has been received, no matter
                    // if the response contains an OAuth success response or an error response
                    DialogResult = DialogResult.OK;
                    Close();
                }));
            }

            return false;
        }

        #endregion

        #region Private CefSharp request handler implementation

        /// <summary>
        /// Implementation of CefSharp request handler, used for capturing the OAuth response.
        /// </summary>
        private class AuthzRequestHandler : IRequestHandler
        {
            private readonly WebBrowserForm parent;

            public AuthzRequestHandler(WebBrowserForm parent)
            {
                this.parent = parent;
            }

            public bool CanGetCookies(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
            {
                return true;
            }

            public bool CanSetCookie(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
            {
                return true;
            }

            public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
            {
                return false;
            }

            public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return null;
            }

            public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
            {
                return false;
            }

            public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
            {
                return CefReturnValue.Continue;
            }

            public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
            {
                return false;
            }

            public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
            {
                return true;
            }

            public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
            {
            }

            public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
            {
                return parent.HandleBrowserProtocolExecution(browserControl, browser, url);
            }

            public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
            {
                return false;
            }

            public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
            {
            }

            public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
            {
            }

            public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
            {
            }

            public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
            {
            }

            public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return false;
            }

            public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
            {
                return false;
            }
        }

        #endregion
    }
}
