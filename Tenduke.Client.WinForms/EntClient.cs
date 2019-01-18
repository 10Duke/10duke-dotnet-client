using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenduke.Client.Authorization;
using Tenduke.Client.Desktop;
using Tenduke.Client.WinForms.Authorization;

namespace Tenduke.Client.WinForms
{
    /// <summary>
    /// Basic client for working directly against the 10Duke Entitlement service.
    /// This client uses the OAuth 2.0 Authorization Code Grant flow for authorizing
    /// this client directly against the 10Duke Entitlement service. If user interaction
    /// is required, a WinForms embedded browser form is displayed.
    /// </summary>
    public class EntClient : BaseDesktopClient<EntClient>
    {
        #region Methods

        /// <summary>
        /// Starts the authorization process and waits for the process to complete before returning.
        /// When authorization has been completed, the <see cref="Authorization"/> property is populated
        /// and the access token in <see cref="AuthorizationInfo.AccessTokenResponse"/> is used for the
        /// subsequent API requests.
        /// </summary>
        public void AuthorizeSync()
        {
            if (OAuthConfig == null)
            {
                throw new InvalidOperationException("OAuthConfig must be specified");
            }

            var authorization = new AuthorizationCodeGrant() { OAuthConfig = OAuthConfig };
            var args = new AuthorizationCodeGrantArgs();
            authorization.AuthorizeSync(args);
            Authorization = authorization;
        }

        #endregion
    }
}
