using System;
using System.Collections.Specialized;
using System.IO;
using Tenduke.Client.Authorization;

namespace Tenduke.Client.DefaultBrowser.Authorization
{
    /// <summary>
    /// Resolves response to be sent to browser when OIDC authentication / authorization
    /// process has been completed.
    /// </summary>
    public interface IAuthorizationCompletedResponseResolver
    {
        /// <summary>
        /// Called for getting HTML response to write to browser after OIDC authentication / authorization
        /// response from the server has been handled.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="authzUri">The full Uri used for calling the authorization endpoint.</param>
        /// <param name="responseParameters">Response parameters received in the response from the server.</param>
        /// <returns><see cref="TextReader"/> for reading the HTML to write in the response.</returns>
        TextReader GetAuthorizationCompletedHtml(
            AuthorizationCodeGrantArgs args,
            Uri authzUri,
            NameValueCollection responseParameters); 
    }
}
