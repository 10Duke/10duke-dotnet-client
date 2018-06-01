using System;
using System.IO;
using System.Reflection;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Default resolver for finding CefSharp unmanaged assemblies.
    /// </summary>
    public class CefSharpResolver
    {
        /// <summary>
        /// Name of subdirectory for resources for 64-bit architecture.
        /// </summary>
        public static readonly string SUBDIR_X64 = "x64";

        /// <summary>
        /// Name of subdirectory for resources for 32-bit architecture.
        /// </summary>
        public static readonly string SUBDIR_X86 = "x86";

        /// <summary>
        /// <para>Attempts to load missing assembly from either <c>x86</c> or <c>x64</c> subdir.
        /// Required by CefSharp to load the unmanaged dependencies when running using <c>AnyCPU</c>.</para>
        /// <para>This overload uses the default settings, using
        /// <see cref="AppDomain.CurrentDomain.SetupInformation.ApplicationBase"/> as the base directory
        /// under which the architecture dependent subdirectories must be found.</para>
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Event arguments.</param>
        /// <returns>Assembly for the correct architecture.</returns>
        public static Assembly ResolveCefSharp(object sender, ResolveEventArgs eventArgs)
        {
            return ResolveCefSharp(
                sender,
                eventArgs,
                new CefSharpResolverArgs { BaseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) });
        }

        /// <summary>
        /// Attempts to load missing assembly from either <c>x86</c> or <c>x64</c> subdir.
        /// Required by CefSharp to load the unmanaged dependencies when running using <c>AnyCPU</c>.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="eventArgs">Event arguments.</param>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched.
        /// Must not be <c>null</c>.</param>
        /// <returns>Assembly for the correct architecture.</returns>
        public static Assembly ResolveCefSharp(object sender, ResolveEventArgs eventArgs, CefSharpResolverArgs resolverArgs)
        {
            if (eventArgs.Name.StartsWith("CefSharp"))
            {
                string assemblyName = eventArgs.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(
                    resolverArgs.BaseDir,
                    Environment.Is64BitProcess ? SUBDIR_X64 : SUBDIR_X86,
                    assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }
}
