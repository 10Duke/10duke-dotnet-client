using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Utility for using the HTML file to use for displaying a loader page in the embedded browser.
    /// </summary>
    public static class LoaderFileUtil
    {
        /// <summary>
        /// Gets stream for reading the loader HTML file.
        /// </summary>
        /// <returns>Stream for reading the file. Caller of this method is responsible for closing the stream.</returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Stream GetLoaderHtmlStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Tenduke.Client.Desktop.resources.loader.html");
        }

        /// <summary>
        /// Writes the loader HTML to a file with the given path.
        /// </summary>
        /// <param name="path">The file path.</param>
        public static void WriteLoaderHtmlToFile(string path)
        {
            using (var loaderHtmlStream = GetLoaderHtmlStream())
            {
                using (var fileStream = File.Create(path))
                {
                    loaderHtmlStream.CopyTo(fileStream);
                }
            }
        }

        /// <summary>
        /// Writes the loader HTML file to a new temp file.
        /// </summary>
        /// <returns>The temp file path.</returns>
        public static string WriteLoaderHtmlToTempFile()
        {
            var retValue = Path.GetTempFileName() + ".html";
            WriteLoaderHtmlToFile(retValue);
            return retValue;
        }
    }
}
