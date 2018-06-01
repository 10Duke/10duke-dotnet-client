using CefSharp;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Utilities for using the CefSharp embedded browser component.
    /// </summary>
    public static class CefSharpUtil
    {
        /// <summary>
        /// <para>Initializes the CefSharp component, loading embedded browser resources for the
        /// correct architecture. This method (any overload of the method) must be called before
        /// using the embedded browser component.</para>
        /// <para>This overload uses the default settings, using
        /// <see cref="AppDomain.CurrentDomain.SetupInformation.ApplicationBase"/> as the base directory
        /// under which the architecture dependent CefSharp resource subdirectories must be found.</para>
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeCefSharp()
        {
            InitializeCefSharp(new CefSharpResolverArgs { BaseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) });
        }

        /// <summary>
        /// Initializes the CefSharp component, loading embedded browser resources for the
        /// correct architecture. This method (any overload of the method) must be called before
        /// using the embedded browser component.
        /// </summary>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched. Must not be <c>null</c>.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeCefSharp(CefSharpResolverArgs resolverArgs)
        {
            var settings = new CefSettings()
            {
                BrowserSubprocessPath = Path.Combine(resolverArgs.BaseDir,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   "CefSharp.BrowserSubprocess.exe")
            };
            Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);
        }

        /// <summary>
        /// Shuts down the CefSharp component. This method must be called to clean up the embedded
        /// browser component.
        /// </summary>
        public static void ShutdownCefSharp()
        {
            Cef.Shutdown();
        }
    }
}
