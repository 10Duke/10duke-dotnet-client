namespace Tenduke.Client.Desktop.Util
{
    /// <summary>
    /// Arguments for resolving CefSharp / cef resources.
    /// </summary>
    public class CefSharpResolverArgs
    {
        /// <summary>
        /// Base directory under which architecture dependent resource directories are located.
        /// If <c>null</c>, CefSharp will try to initialize with defaults.
        /// </summary>
        public string BaseDir { get; set; }
    }
}
