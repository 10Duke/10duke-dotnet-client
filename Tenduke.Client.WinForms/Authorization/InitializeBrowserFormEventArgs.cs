namespace Tenduke.Client.WinForms.Authorization
{
    public class InitializeBrowserFormEventArgs
    {
        /// <summary>
        /// The <see cref="WebBrowserForm"/> to initialize.
        /// Event subscribers may set properties of the form.
        /// </summary>
        public WebBrowserForm WebBrowserForm { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializeBrowserFormEventArgs"/> class.
        /// </summary>
        /// <param name="webBrowserForm">The <see cref="WebBrowserForm"/> to initialize.
        /// Event subscribers may set properties of the form.</param>
        public InitializeBrowserFormEventArgs(WebBrowserForm webBrowserForm)
        {
            WebBrowserForm = webBrowserForm;
        }
    }
}
