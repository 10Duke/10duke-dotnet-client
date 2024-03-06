using System;
using System.Windows.Forms;

namespace Tenduke.Client.WinFormsSample.NetFramework
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var appContext = new WinFormsSampleApplicationContext();
            Application.Run(appContext);
        }
    }
}
