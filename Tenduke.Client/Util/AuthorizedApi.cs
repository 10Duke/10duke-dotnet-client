using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Tenduke.Client.Util
{
    /// <summary>
    /// Base class for API classes that use OAuth authorization.
    /// </summary>
    public abstract class AuthorizedApi
    {
        #region Properties

        /// <summary>
        /// Access token from the 10Duke Identity and Entitlement service,
        /// for authorizing requests to the API.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// The <see cref="HttpClient"/> to use for calling the API.
        /// </summary>
        public HttpClient HttpClient { get; set; }

        #endregion
    }
}
