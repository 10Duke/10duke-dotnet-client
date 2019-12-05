using System;
using System.Windows;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.WPF;

namespace Tenduke.Client.WPFSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var resolverArgs = CefSharpUtil.AddAssemblyResolverForCefSharp();
            WPF.EntClient.Initialize(resolverArgs);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            WPF.EntClient.Shutdown();
        }
    }
}
