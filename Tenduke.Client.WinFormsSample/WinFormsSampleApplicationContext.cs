using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.WinForms;

namespace Tenduke.Client.WinFormsSample
{
    /// <summary>
    /// Application context in which this sample application is executed. Application-wide static
    /// initialization and clean-up is done by this class.
    /// </summary>
    class WinFormsSampleApplicationContext : ApplicationContext
    {
        /// <summary>
        /// Main form of the application.
        /// </summary>
        private MainForm mainForm;

        /// <summary>
        /// <para>Initializes a new instance of the <see cref="WinFormsSampleApplicationContext"/> class.</para>
        /// <para><see cref="EntClient.Initialize"/> is called here for initializing <see cref="EntClient"/>.
        /// This method must be called once before using <see cref="EntClient"/> in the application.</para>
        /// </summary>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched,
        /// or <c>null</c> for default behavior.</param>
        internal WinFormsSampleApplicationContext(CefSharpResolverArgs resolverArgs)
        {
            Application.ApplicationExit += Application_ApplicationExit;

            // Application-wide static initialization of EntClient
            EntClient.Initialize(resolverArgs);

            mainForm = new MainForm();
            mainForm.FormClosed += MainForm_FormClosed;
            mainForm.Show();
        }

        /// <summary>
        /// Called when the main form of the application is closed.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainForm.Dispose();
            mainForm = null;
            Application.Exit();
        }

        /// <summary>
        /// <para>Called when the application is about to shut down.</para>
        /// <para><see cref="EntClient.Shutdown"/> is called here for cleaning up resources used by <see cref="EntClient"/>.
        /// This method must be called once when <see cref="EntClient"/> is not anymore used by the application.</para>
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            // Application-wide static clean-up of EntClient
            EntClient.Shutdown();
        }
    }
}
