using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tenduke.Client.WPF
{
    /// <summary>
    /// Interaction logic for WebBrowserWindow.xaml
    /// </summary>
    public partial class WebBrowserWindow : Window
    {
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

        public WebBrowserWindow()
        {
            InitializeComponent();
        }

        #endregion
    }
}
