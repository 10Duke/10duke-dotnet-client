using CefSharp;
using CefSharp.WinForms;
using System;
using Tenduke.Client.Authorization;
using Tenduke.Client.Desktop;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.WinForms.Authorization;

namespace Tenduke.Client.WinForms
{
    /// <summary>
    /// Basic client for working directly against the 10Duke Entitlement service.
    /// This client uses the OAuth 2.0 Authorization Code Grant flow for authorizing
    /// this client directly against the 10Duke Entitlement service. If user interaction
    /// is required, a WinForms embedded browser form is displayed.
    /// </summary>
    public class EntClient : BaseDesktopClient
    {

        #region Events

        /// <summary>
        /// Raised when initializing the web browser form. Subscribers of the event may
        /// set properties of the form.
        /// </summary>
        public event EventHandler<InitializeBrowserFormEventArgs> RaiseInitializeBrowserForm;

        #endregion

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
        public static void Initialize()
        {
#if !NET5_0_OR_GREATER
            CefSharpUtil.InitializeAssemblyResolver();
#endif
            var cefSettings = BuildDefaultCefSettings();
            Initialize(cefSettings);
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
        /// <remarks>
        /// To call this method, the application needs to have created the CefSettings object, which means that
        /// CefSharp.Core.Runtime assembly will already have been resolved.
        /// For .NET Framework, this will require the application to have already called CefSharp
        /// </remarks>
        public static void Initialize(CefSettings cefSettings)
        {
            BaseDesktopClient.Initialize(cefSettings);
        }

        /// <summary>
        /// Builds cef settings object and populates it with default settings.
        /// </summary>
        /// <returns>The initialized CefSharp initialization parameter object.</returns>
        public static CefSettings BuildDefaultCefSettings()
        {
            var retValue = new CefSettings();
            PopulateDefaultCefSettings(retValue);
            return retValue;
        }

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

            var retValue = new AuthorizationCodeGrant() { OAuthConfig = OAuthConfig };
            retValue.RaiseInitializeBrowserForm += HandleRaiseInitializeBrowserForm;
            return retValue;
        }

        /// <summary>
        /// Called for invoking the <see cref="RaiseInitializeBrowserForm"/> event.
        /// </summary>
        /// <param name="e">The <see cref="InitializeBrowserFormEventArgs"/>.</param>
        protected virtual void OnRaiseInitializeBrowserForm(InitializeBrowserFormEventArgs e)
        {
            RaiseInitializeBrowserForm?.Invoke(this, e);
        }

        private void HandleRaiseInitializeBrowserForm(object sender, InitializeBrowserFormEventArgs e)
        {
            OnRaiseInitializeBrowserForm(e);
        }

        /// <summary>
        /// Clears cookies of the built-in browser component. Note that this clears cookies globally
        /// for all Cef-based browsers.
        /// </summary>
        public static void ClearCookies()
        {
            Cef.GetGlobalCookieManager().DeleteCookies("", "");
        }

        #endregion
    }
}
