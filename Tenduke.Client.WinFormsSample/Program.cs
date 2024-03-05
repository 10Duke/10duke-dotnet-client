using CefSharp.WinForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.WinForms;

namespace Tenduke.Client.WinFormsSample
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
