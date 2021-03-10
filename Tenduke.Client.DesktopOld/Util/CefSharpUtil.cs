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
        /// Builds a <see cref="CefSharpResolverArgs"/> argument object for CefSharp initialization with
        /// default <see cref="CefSharpResolverArgs.BaseDir"/> value used for finding CefSharp assemblies
        /// and binaries.
        /// </summary>
        /// <returns>The default <see cref="CefSharpResolverArgs"/> object.</returns>
        public static CefSharpResolverArgs BuildDefaultCefSharpResolverArgs()
        {
            return new CefSharpResolverArgs
            {
                BaseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)
            };
        }

        /// <summary>
        /// Adds a <see cref="CefSharpResolver"/> as an <see cref="AppDomain.AssemblyResolve"/> handler of the <see cref="AppDomain.CurrentDomain"/>.
        /// This is necessary for finding CefSharp assemblies.
        /// </summary>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched, or <c>null</c> to use defaults.</param>
        /// <returns>The <see cref="CefSharpResolverArgs"/> object used for initializing the resolver.</returns>
        public static CefSharpResolverArgs AddAssemblyResolverForCefSharp(CefSharpResolverArgs resolverArgs = null)
        {
            var resolverArgsOrDefault = resolverArgs == null ? BuildDefaultCefSharpResolverArgs() : resolverArgs;
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) => CefSharpResolver.ResolveCefSharp(sender, eventArgs, resolverArgsOrDefault);
            return resolverArgsOrDefault;
        }

        /// <summary>
        /// <para>Initializes the CefSharp component, loading embedded browser resources for the
        /// correct architecture. This method (any overload of the method) must be called before
        /// using the embedded browser component.</para>
        /// <para>This overload uses the default settings, using
        /// <see cref="AppDomain.CurrentDomain.SetupInformation.ApplicationBase"/> as the base directory
        /// under which the architecture dependent CefSharp resource subdirectories must be found.</para>
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameters. In many cases it is sufficient to
        /// pass an empty instance of a derived class suitable for the use case. Must not be <c>null</c>.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeCefSharp(CefSettingsBase cefSettings)
        {
            InitializeCefSharp(cefSettings, BuildDefaultCefSharpResolverArgs());
        }

        /// <summary>
        /// Initializes the CefSharp component, loading embedded browser resources for the
        /// correct architecture. This method (any overload of the method) must be called before
        /// using the embedded browser component.
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameters. In many cases it is sufficient to
        /// pass an empty instance of a derived class suitable for the use case. Must not be <c>null</c>.</param>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched,
        /// or <c>null</c> for default behavior.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeCefSharp(CefSettingsBase cefSettings, CefSharpResolverArgs resolverArgs = null)
        {
            var resolverArgsOrDefault = resolverArgs == null ? BuildDefaultCefSharpResolverArgs() : resolverArgs;
            if (resolverArgsOrDefault.BaseDir != null)
            {
                var browserSubprocessFileName = "CefSharp.BrowserSubprocess.exe";
                var archSpecificBrowserSubprocessPath = Path.Combine(resolverArgsOrDefault.BaseDir,
                                                   Environment.Is64BitProcess ? "x64" : "x86",
                                                   browserSubprocessFileName);
                if (File.Exists(archSpecificBrowserSubprocessPath))
                {
                    cefSettings.BrowserSubprocessPath = archSpecificBrowserSubprocessPath;
                }
                else
                {
                    var flatBrowserSubprocessPath = Path.Combine(resolverArgsOrDefault.BaseDir,
                                                       browserSubprocessFileName);
                    cefSettings.BrowserSubprocessPath = flatBrowserSubprocessPath;
                }
            }
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
