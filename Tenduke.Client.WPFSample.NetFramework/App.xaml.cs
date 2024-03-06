using System.Windows;
using Tenduke.Client.Desktop.Util;

namespace Tenduke.Client.WPFSample.NetFramework
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CefSharpUtil.InitializeAssemblyResolver();
            var cefSettings = WPF.EntClient.BuildDefaultCefSettings();
            cefSettings.IgnoreCertificateErrors = true;
            WPF.EntClient.Initialize(cefSettings);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            WPF.EntClient.Shutdown();
        }
    }
}
