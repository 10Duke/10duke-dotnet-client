using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using Tenduke.Client.Config;

namespace Tenduke.Client.Authorization
{
    /// <summary>
    /// Base class for authorization implementations, containing the authorization result data.
    /// </summary>
    [Serializable]
    public abstract class AuthorizationInfo
    {
        #region Properties

        /// <summary>
        /// OAuth 2.0 Access Token response received from the 10Duke Identity and Entitlement service,
        /// or <c>null</c> if no access token currently obtained.
        /// </summary>
        public AccessTokenResponse AccessTokenResponse { get; set; }

        /// <summary>
        /// Error code received from the 10Duke Identity and Entitlement service, or <c>null</c> if there is no error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Error description received from the 10Duke Identity and Entitlement service.
        /// The error description may be given in the case that <see cref="Error"/> is not <c>null</c>.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Error Uri for additional error information, received from the 10Duke Identity and Entitlement service.
        /// The error Uri may be given in the case that <see cref="Error"/> is not <c>null</c>.
        /// </summary>
        public string ErrorUri { get; set; }

        /// <summary>
        /// Timestamp when this authorization info has been received from the 10Duke Identity and Entitlement service.
        /// </summary>
        public DateTime? Received { get; set; }

        #endregion
    }

    /// <summary>
    /// Base class for authorization implementations.
    /// </summary>
    /// <typeparam name="T">Type of the implementing class.</typeparam>
    /// <typeparam name="O">OAuth 2.0 configuration object type.</typeparam>
    /// <typeparam name="A">Authorization process argument type.</typeparam>
    [Serializable]
    public abstract class Authorization<T, O, A> : AuthorizationInfo, IDisposable
            where T : Authorization<T, O, A>
            where O : IOAuthConfig
            where A : AuthorizationArgs
    {
        #region Event handlers

        /// <summary>
        /// Occurs when authorization process has been triggered.
        /// </summary>
        public event EventHandler<AuthorizationStartedEventArgs<O>> Started;

        /// <summary>
        /// Occurs just before sending request to the authorization endpoint of the 10Duke Entitlement service.
        /// This event is triggered with OAuth 2.0 flows that call the authorization endpoint,
        /// i.e. with the Authorization Code Grant flow and the Implicit Grant flow.
        /// </summary>
        public event EventHandler<BeforeAuthorizationEventArgs> BeforeAuthorization;

        /// <summary>
        /// <para>Occurs just after executing request to the authorization endpoint of the 10Duke Entitlement service
        /// is completed and a response has been received. The response may be a success response or an error
        /// response.</para>
        /// <para>If the process is interrupted by the user, <see cref="Cancelled"/> is triggered instead of this event.</para>
        /// <para>This event is triggered with OAuth 2.0 flows that call the authorization endpoint,
        /// i.e. with the Authorization Code Grant flow and the Implicit Grant flow.</para>
        /// </summary>
        public event EventHandler<AfterAuthorizationEventArgs> AfterAuthorization;

        /// <summary>
        /// Occurs when authorization process has been interrupted by the end user.
        /// </summary>
        public event EventHandler<AuthorizationCancelledEventArgs> Cancelled;

        /// <summary>
        /// Occurs when authorization process has completed successfully and an access token response has been
        /// received from the 10Duke Entitlement service.
        /// </summary>
        public event EventHandler<AuthorizationCompletedEventArgs> Completed;

        /// <summary>
        /// Occurs when authorization process has failed and an error response has been
        /// received from the 10Duke Entitlement service.
        /// </summary>
        public event EventHandler<AuthorizationFailedEventArgs> Failed;

        #endregion

        #region Properties

        /// <summary>
        /// OAuth 2.0 configuration used for the authorization.
        /// </summary>
        public O OAuthConfig { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the authorization process and waits for the process to complete before returning.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <returns>Returns OAuth state as received back from the server, or <c>null</c> if OAuth state
        /// is not used with the authorization flow, or if no OAuth state specified in the <paramref name="args"/>.</returns>
        public abstract string AuthorizeSync(A args);

        /// <summary>
        /// Reads response and populates this object from the response parameters received
        /// from the server as a response to an authorization request.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="responseParameters">The response parameters from the server.</param>
        /// <returns>Returns the OAuth state read from the response parameters, or <c>null</c> if no <c>state</c>
        /// response parameter found.</returns>
        protected virtual string ReadAuthorizationResponse(A args, NameValueCollection responseParameters)
        {
            Error = responseParameters["error"];
            ErrorDescription = responseParameters["error_description"];
            ErrorUri = responseParameters["error_uri"];
            return responseParameters["state"];
        }

        /// <summary>
        /// Parses response from the server to an access token request, and populates fields of this object.
        /// </summary>
        /// <param name="accessTokenResponse">JSON string response received from the server.</param>
        protected void ReadAccessTokenResponse(string accessTokenResponse)
        {
            dynamic json = JsonConvert.DeserializeObject(accessTokenResponse);
            ReadAccessTokenResponse(json);
        }

        /// <summary>
        /// Parses response from the server to an access token request, and populates fields of this object.
        /// </summary>
        /// <param name="accessTokenResponse">Dynamic object representing the JSON response received from the server.</param>
        protected abstract void ReadAccessTokenResponse(dynamic accessTokenResponse);

        /// <summary>
        /// Parses response from the server to an access token request, and populates fields of this object.
        /// </summary>
        /// <param name="accessTokenResponse">Dynamic object representing the JSON response received from the server.</param>
        protected void ReadAccessTokenResponseCommon(dynamic accessTokenResponse)
        {
            Error = accessTokenResponse["error"];
            ErrorDescription = accessTokenResponse["error_description"];
            ErrorUri = accessTokenResponse["error_uri"];
            Received = DateTime.UtcNow;
        }

        /// <summary>
        /// Triggers the <see cref="Started"/> event.
        /// </summary>
        protected void OnStarted()
        {
            Started?.Invoke(this, new AuthorizationStartedEventArgs<O>()
            {
                OAuthConfig = OAuthConfig
            });
        }

        /// <summary>
        /// Triggers the <see cref="BeforeAuthorization"/> event.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="authzUri">The full Uri used for calling the authorization endpoint.</param>
        protected void OnBeforeAuthorization(A args, Uri authzUri)
        {
            BeforeAuthorization?.Invoke(this, new BeforeAuthorizationEventArgs()
            {
                AuthzArgs = args,
                AuthzUri = authzUri
            });
        }

        /// <summary>
        /// Triggers the <see cref="AfterAuthorization"/> event.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="authzUri">The full Uri used for calling the authorization endpoint.</param>
        /// <param name="responseParameters">Response parameters received in the response from the server.</param>
        protected void OnAfterAuthorization(A args, Uri authzUri, NameValueCollection responseParameters)
        {
            AfterAuthorization?.Invoke(this, new AfterAuthorizationEventArgs()
            {
                AuthzArgs = args,
                AuthzUri = authzUri,
                ResponseParameters = responseParameters
            });
        }

        /// <summary>
        /// Triggers the <see cref="Cancelled"/> event.
        /// </summary>
        /// <param name="args">Authorization operation arguments.</param>
        /// <param name="authzUri">The full Uri used for calling the authorization endpoint.</param>
        protected void OnCancelled(A args, Uri authzUri)
        {
            Cancelled?.Invoke(this, new AuthorizationCancelledEventArgs()
            {
                AuthzArgs = args,
                AuthzUri = authzUri
            });
        }

        /// <summary>
        /// Triggers the <see cref="Completed"/> event.
        /// </summary>
        protected void OnCompleted()
        {
            Completed?.Invoke(this, new AuthorizationCompletedEventArgs()
            {
                AccessTokenResponse = AccessTokenResponse
            });
        }

        /// <summary>
        /// Triggers the <see cref="Failed"/> event.
        /// </summary>
        protected void OnFailed()
        {
            Failed?.Invoke(this, new AuthorizationFailedEventArgs()
            {
                Error = Error,
                ErrorDescription = ErrorDescription,
                ErrorUri = ErrorUri
            });
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Frees any resources consumed by the <see cref="Authorization"/> object. Override
        /// this method in derived classes if there are resources that need bo be disposed.
        /// </summary>
        /// <param name="disposing"><c>true</c> if called from the zero-argument <see cref="Dispose"/> method,
        /// <c>false</c> if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // No managed resources to free
                }

                // No unmanaged resources to free

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
