namespace Tenduke.Client.WPF.Authorization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InitializeBrowserWindowEventArgs"/> class.
    /// </summary>
    /// <param name="webBrowserWindow">The <see cref="WebBrowserWindow"/> to initialize.
    /// Event subscribers may set properties of the window.</param>
    public class InitializeBrowserWindowEventArgs(WebBrowserWindow webBrowserWindow)
    {
        /// <summary>
        /// The <see cref="WebBrowserWindow"/> to initialize.
        /// Event subscribers may set properties of the window.
        /// </summary>
        public WebBrowserWindow WebBrowserWindow { get; set; } = webBrowserWindow;
    }
}
