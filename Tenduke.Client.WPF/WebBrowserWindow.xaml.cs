﻿using CefSharp;
using CefSharp.Enums;
using CefSharp.Structs;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using Tenduke.Client.Desktop.Util;

namespace Tenduke.Client.WPF
{
    /// <summary>
    /// Interaction logic for WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window
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

        /// <summary>
        /// Indicates if the window is in closed state.
        /// </summary>
        private bool closed;

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

        /// <summary>
        /// Indicates if insecure certificates are accepted.
        /// </summary>
        public bool AllowInsecureCerts { get; set; }

        /// <summary>
        /// Indicates if CEF console logging is enabled.
        /// </summary>
        public bool EnableCefConsoleLogging { get; set; }

        #endregion

        #region Constructors

        public WebBrowserWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Called when the web browser window is shown.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void windowWebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            closed = false;
            if (loaderPath == null)
            {
                loaderPath = LoaderFileUtil.WriteLoaderHtmlToTempFile();
            }
            ChromiumWebBrowser = new ChromiumWebBrowser
            {
                Address = loaderPath,
                RequestHandler = new AuthzRequestHandler(this),
                DisplayHandler = new CefDisplayHandler(this),
            };
            initialPageLoadStarted = false;
            ChromiumWebBrowser.LoadingStateChanged += ChromiumWebBrowser_LoadingStateChanged;
            gridForWebBrowser.Children.Add(ChromiumWebBrowser);
        }

        private void ChromiumWebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!initialPageLoadStarted && !e.IsLoading)
            {
                initialPageLoadStarted = true;
                ChromiumWebBrowser.Load(Address);
            }
        }

        private void windowWebBrowser_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            closed = true;
            if (ChromiumWebBrowser != null)
            {
                gridForWebBrowser.Children.Remove(ChromiumWebBrowser);
                ChromiumWebBrowser.Dispose();
                ChromiumWebBrowser = null;
            }

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
                CloseInternal();
            }

            return false;
        }

        private void CloseInternal()
        {
            Task.Delay(100).ContinueWith(t =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (!closed)
                    {
                        // true indicates that response has been received, no matter
                        // if the response contains an OAuth success response or an error response
                        DialogResult = true;
                        Close();
                    }
                }));
            });
        }

        #endregion

        #region Private CefSharp request handler implementation

        /// <summary>
        /// Implementation of CefSharp resource request handler, used for capturing the OAuth response.
        /// </summary>
        private class AuthzResourceRequestHandler : IResourceRequestHandler
        {
            private readonly WebBrowserWindow parent;
            private bool disposedValue;

            public AuthzResourceRequestHandler(WebBrowserWindow parent)
            {
                this.parent = parent;
            }

            public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
            {
                return null;
            }

            public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
            {
                return null;
            }

            public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return null;
            }

            public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
            {
                return CefReturnValue.Continue;
            }

            public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
            {
                return parent.HandleBrowserProtocolExecution(chromiumWebBrowser, browser, request.Url);
            }

            public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
            {
            }

            public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
            {
            }

            public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
            {
                return false;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~AuthzResourceRequestHandler()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Implementation of CefSharp request handler, used for capturing the OAuth response.
        /// </summary>
        private class AuthzRequestHandler : IRequestHandler
        {
            private readonly WebBrowserWindow parent;

            public AuthzRequestHandler(WebBrowserWindow parent)
            {
                this.parent = parent;
            }

            public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
            {
                return true;
            }

            public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
            {
                return new AuthzResourceRequestHandler(parent);
            }

            public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
            {
                return false;
            }

            public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
            {
                if (!parent.AllowInsecureCerts)
                {
                    return false;
                }

                callback.Continue(true);
                return true;
            }

            public void OnDocumentAvailableInMainFrame(IWebBrowser chromiumWebBrowser, IBrowser browser)
            {
            }

            public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
            {
                return true;
            }

            public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
            {
            }

            public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
            {
                return false;
            }

            public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
            {
            }

            public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status, int errorCode, string errorMessage)
            {
            }

            public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
            {
            }

            public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
            {
                return false;
            }
        }

        #endregion

        #region Private CefSharp display handler implementation

        private class CefDisplayHandler : IDisplayHandler
        {
            private readonly WebBrowserWindow parent;

            public CefDisplayHandler(WebBrowserWindow parent)
            {
                this.parent = parent;
            }
            public void OnAddressChanged(IWebBrowser chromiumWebBrowser, AddressChangedEventArgs addressChangedArgs)
            {
            }

            public bool OnAutoResize(IWebBrowser chromiumWebBrowser, IBrowser browser, CefSharp.Structs.Size newSize)
            {
                return false;
            }

            public bool OnConsoleMessage(IWebBrowser chromiumWebBrowser, ConsoleMessageEventArgs consoleMessageArgs)
            {
                // Return true to stop the message from being output to the console.
                return !parent.EnableCefConsoleLogging;
            }

            public bool OnCursorChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IntPtr cursor, CursorType type, CursorInfo customCursorInfo)
            {
                return false;
            }

            public void OnFaviconUrlChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls)
            {
            }

            public void OnFullscreenModeChange(IWebBrowser chromiumWebBrowser, IBrowser browser, bool fullscreen)
            {
            }

            public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
            {
            }

            public void OnStatusMessage(IWebBrowser chromiumWebBrowser, StatusMessageEventArgs statusMessageArgs)
            {
            }

            public void OnTitleChanged(IWebBrowser chromiumWebBrowser, TitleChangedEventArgs titleChangedArgs)
            {
            }

            public bool OnTooltipChanged(IWebBrowser chromiumWebBrowser, ref string text)
            {
                return false;
            }
        }

        #endregion
    }
}
