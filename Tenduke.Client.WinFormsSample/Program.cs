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
            var resolverArgs = new CefSharpResolverArgs() { BaseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) };
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) => CefSharpResolver.ResolveCefSharp(sender, eventArgs, resolverArgs);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new WinFormsSampleApplicationContext());
        }
    }
}
