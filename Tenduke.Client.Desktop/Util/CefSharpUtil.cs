using CefSharp;
using System.Runtime.CompilerServices;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Utilities for using the CefSharp embedded browser component.
    /// </summary>
    public static class CefSharpUtil
    {
        /// <summary>
        /// Initializes the CefSharp component, loading embedded browser resources for the
        /// correct architecture. This method must be called before using the embedded browser
        /// component.
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameters. In many cases it is sufficient to
        /// pass an empty instance of a derived class suitable for the use case. Must not be <c>null</c>.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeCefSharp(CefSettingsBase cefSettings)
        {
#if !NET5_0_OR_GREATER
            CefSharp.CefRuntime.SubscribeAnyCpuAssemblyResolver();
#endif
            Cef.Initialize(cefSettings, performDependencyCheck: false, browserProcessHandler: null);
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
