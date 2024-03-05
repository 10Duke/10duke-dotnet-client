using CefSharp;
using System;
using System.IO;
using System.Reflection;
using Tenduke.Client.Config;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.WinBase;

namespace Tenduke.Client.Desktop
{
    /// <summary>
    /// Base class for desktop clients working against the 10Duke Entitlement service and using
    /// the CefSharp embedded browser for user interaction.
    /// </summary>
    public class BaseDesktopClient : BaseWinClient<IAuthorizationCodeGrantConfig>
    {
        #region Private fields

        /// <summary>
        /// Internal flag tracking if CefSharp initialization has been done.
        /// </summary>
        private static bool cefSharpInitialized;

        #endregion

        #region Application-wide initialization and clean-up

        /// <summary>
        /// <para>Executes initialization necessary for using <see cref="BaseDesktopClient{C}"/> in an application.
        /// This method (any overload of this method) must be called once during application lifecycle,
        /// before using <see cref="EntClient"/> for the first time.</para>
        /// <para><see cref="EntClient"/> uses <see cref="https://github.com/cefsharp/CefSharp"/> for displaying
        /// an embedded browser window for operations that require displaying server-side user interface,
        /// especially a sign-on window. <see cref="EntClient"/> supports using <c>AnyCPU</c> as target architecture,
        /// and with <c>CefSharp</c> this means that loading unmanaged CefSharp resources is required. This
        /// method assumes that the required CefSharp dependencies can be found under <c>x84</c> or <c>x64</c>
        /// subdirectories.</para>
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameters. In many cases it is sufficient to
        /// pass an empty instance of a derived class suitable for the use case. Must not be <c>null</c>.</param>
        public static void Initialize(CefSettingsBase cefSettings)
        {
            CefSharpUtil.InitializeCefSharp(cefSettings);
            cefSharpInitialized = true;
        }

        /// <summary>
        /// Populates default settings in the given settings object.
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameter object.</param>
        protected static void PopulateDefaultCefSettings(CefSettingsBase cefSettings)
        {
            cefSettings.CefCommandLineArgs.Remove("enable-system-flash");
            // Disable GPU settings because on some GPUs these cause incorrect rendering when display is scaled
            cefSettings.CefCommandLineArgs.Add("disable-gpu", "1"); // Disable GPU acceleration
            cefSettings.CefCommandLineArgs.Add("disable-gpu-vsync", "1"); // Disable GPU vsync
            cefSettings.CefCommandLineArgs.Add("disable-gpu-shader-disk-cache", "1"); // Do not use GPU disk cache
        }

        /// <summary>
        /// Cleans up resources required for using <see cref="BaseDesktopClient{C}"/> in an application.
        /// This method must be called once when <see cref="BaseDesktopClient{C}"/> is no more used by the application.
        /// </summary>
        public static void Shutdown()
        {
            if (cefSharpInitialized)
            {
                CefSharpUtil.ShutdownCefSharp();
                cefSharpInitialized = false;
            }
        }

        #endregion
    }
}
