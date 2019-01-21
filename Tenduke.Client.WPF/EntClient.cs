using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.Desktop;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.WPF.Authorization;

namespace Tenduke.Client.WPF
{
    /// <summary>
    /// Basic client for working directly against the 10Duke Entitlement service.
    /// This client uses the OAuth 2.0 Authorization Code Grant flow for authorizing
    /// this client directly against the 10Duke Entitlement service. If user interaction
    /// is required, a WPM embedded browser windows is displayed.
    /// </summary>
    public class EntClient : BaseDesktopClient<EntClient>
    {
        #region Methods

        /// <summary>
        /// <para>Executes initialization necessary for using <see cref="EntClient"/> in an application.
        /// This method (any overload of this method) must be called once during application lifecycle,
        /// before using <see cref="EntClient"/> for the first time. This overload executes initialization
        /// with a default <see cref="CefSettings"/> object.</para>
        /// <para><see cref="EntClient"/> uses <see cref="https://github.com/cefsharp/CefSharp"/> for displaying
        /// an embedded browser window for operations that require displaying server-side user interface,
        /// especially a sign-on window. <see cref="EntClient"/> supports using <c>AnyCPU</c> as target architecture,
        /// and with <c>CefSharp</c> this means that loading unmanaged CefSharp resources is required. This
        /// method assumes that the required CefSharp dependencies can be found under <c>x84</c> or <c>x64</c>
        /// subdirectories.</para>
        /// </summary>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched,
        /// or <c>null</c> for default behavior.</param>
        public static void Initialize(CefSharpResolverArgs resolverArgs = null)
        {
            Initialize(new CefSettings(), resolverArgs);
        }

        /// <summary>
        /// <para>Executes initialization necessary for using <see cref="EntClient"/> in an application.
        /// This method (any overload of this method) must be called once during application lifecycle,
        /// before using <see cref="EntClient"/> for the first time.</para>
        /// <para><see cref="EntClient"/> uses <see cref="https://github.com/cefsharp/CefSharp"/> for displaying
        /// an embedded browser window for operations that require displaying server-side user interface,
        /// especially a sign-on window. <see cref="EntClient"/> supports using <c>AnyCPU</c> as target architecture,
        /// and with <c>CefSharp</c> this means that loading unmanaged CefSharp resources is required. This
        /// method assumes that the required CefSharp dependencies can be found under <c>x84</c> or <c>x64</c>
        /// subdirectories.</para>
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameters. In many cases it is sufficient to
        /// pass an empty instance. Must not be <c>null</c>.</param>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched,
        /// or <c>null</c> for default behavior.</param>
        public static void Initialize(CefSettings cefSettings, CefSharpResolverArgs resolverArgs = null)
        {
            BaseDesktopClient<EntClient>.Initialize(cefSettings, resolverArgs);
        }

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
