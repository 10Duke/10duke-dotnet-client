using System;

namespace Tenduke.Client.EntApi
{
    /// <summary>
    /// Content type for returning response from the 10Duke Entitlement service.
    /// </summary>
    public class ResponseType
    {
        /// <summary>
        /// Content type associated with the response type.
        /// </summary>
        public string ContentType { get; protected set; }

        /// <summary>
        /// Extension to use in request paths for requesting a response of this response type.
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseType"/> class.
        /// </summary>
        /// <param name="contentType">Content type associated with the response type.</param>
        /// <param name="extension">Extension to use in request paths for requesting a response of this response type.</param>
        private ResponseType(string contentType, string extension)
        {
            ContentType = contentType;
            Extension = extension;
        }

        /// <summary>
        /// Instruct server to use server-defined default response format.
        /// </summary>
        public static ResponseType Default { get { return new ResponseType(null, ""); } }

        /// <summary>
        /// Instruct server to respond in <c>application/json</c> format.
        /// </summary>
        public static ResponseType Json { get { return new ResponseType("application/json", ".json"); } }

        /// <summary>
        /// Instruct server to respond in <c>application/jwt</c> format.
        /// </summary>
        public static ResponseType JWT { get { return new ResponseType("application/jwt", ".jwt"); } }

        /// <summary>
        /// Instruct server to respond in <c>text/plain</c> format.
        /// </summary>
        public static ResponseType Text { get { return new ResponseType("text/plain", ".txt"); } }

        /// <summary>
        /// Returns a <see cref="ResponseType"/> object for the given request path extension.
        /// </summary>
        /// <param name="extension">Extension to use in request path ("file extension") for requesting
        /// server to respond in desired response content type.</param>
        /// <returns>The <see cref="ResponseType"/> object.</returns>
        public static ResponseType FromExtension(string extension)
        {
            return extension switch
            {
                ".jwt" => ResponseType.JWT,
                ".json" => ResponseType.Json,
                ".txt" => ResponseType.Text,
                "" or null => ResponseType.Default,
                _ => throw new NotSupportedException(string.Format("Extension {0} not supported", extension)),
            };
        }
    }
}
