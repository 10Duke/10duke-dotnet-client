using System;
using System.Runtime.Serialization;

namespace Tenduke.Client.EntApi
{
    /// <summary>
    /// Exception thrown if server returns an invalid response.
    /// </summary>
    public class InvalidServerResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidServerResponseException"/> class.
        /// </summary>
        public InvalidServerResponseException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidServerResponseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidServerResponseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidServerResponseException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected InvalidServerResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidServerResponseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a <c>null</c> reference if no inner exception is specified.</param>
        public InvalidServerResponseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
