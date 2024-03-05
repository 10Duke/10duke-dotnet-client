using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tenduke.Client.Authorization;
using Tenduke.Client.DefaultBrowser.Authorization;
using Tenduke.Client.DefaultBrowser.Config;
using Tenduke.Client.WinBase;

namespace Tenduke.Client.DefaultBrowser
{
    /// <summary>
    /// Client for working against the 10Duke Entitlement service.
    /// This client uses the OAuth 2.0 Authorization Code Grant flow (with PKCE)
    /// for authorizing this client against the 10Duke Entitlement service.
    /// If user interaction is required, the operating system default browser
    /// is used.
    /// </summary>
    public class EntClient : BaseWinClient<IDefaultBrowserAuthorizationCodeGrantConfig>
    {
        #region Properties

        /// <summary>
        /// Resolver for HTML response to send after OIDC authentication / authorization
        /// process has been completed. If not specified, default HTML response is returned.
        /// </summary>
        public IAuthorizationCompletedResponseResolver AuthorizationCompletedResponseResolver { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the authorization process and waits for the process to complete before returning.
        /// When authorization has been completed, the <see cref="Authorization"/> property is populated
        /// and the access token in <see cref="AuthorizationInfo.AccessTokenResponse"/> is used for the
        /// subsequent API requests.
        /// </summary>
        public virtual void AuthorizeSync()
        {
            var authorization = InitializeAuthorizationCodeGrant();
            var args = new AuthorizationCodeGrantArgs();
            if (OAuthConfig.UsePkce)
            {
                args = args.WithNewCodeVerifier();
            }
            authorization.AuthorizeSync(args);
            Authorization = authorization;
        }

        /// <summary>
        /// Starts the authorization process.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token for cancelling the operation.</param>
        public virtual async Task Authorize(CancellationToken? cancellationToken = null)
        {
            var authorization = InitializeAuthorizationCodeGrant();
            var args = new AuthorizationCodeGrantArgs();
            if (OAuthConfig.UsePkce)
            {
                args = args.WithNewCodeVerifier();
            }
            await authorization.Authorize(args, cancellationToken);
            Authorization = authorization;
        }

        /// <summary>
        /// Creates and initializes the <see cref="AuthorizationCodeGrant"/> object that is used
        /// for executing the OAuth 2.0 authorization code grant flow using an embedded browser.
        /// </summary>
        /// <returns>The <see cref="AuthorizationCodeGrant"/> object.</returns>
        protected virtual AuthorizationCodeGrant InitializeAuthorizationCodeGrant()
        {
            if (OAuthConfig == null)
            {
                throw new InvalidOperationException("OAuthConfig must be specified");
            }

            return new AuthorizationCodeGrant()
            {
                OAuthConfig = OAuthConfig,
                AuthorizationCompletedResponseResolver = AuthorizationCompletedResponseResolver
            };
        }

        #endregion
    }
}
