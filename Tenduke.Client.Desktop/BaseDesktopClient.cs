using CefSharp;
using System;
using System.IO;
using System.Reflection;
using Tenduke.Client.Authorization;
using Tenduke.Client.Config;
using Tenduke.Client.Desktop.Config;
using Tenduke.Client.Desktop.Util;
using Tenduke.Client.EntApi.Authz;

namespace Tenduke.Client.Desktop
{
    /// <summary>
    /// Base class for desktop clients working against the 10Duke Entitlement service and using
    /// the CefSharp embedded browser for user interaction.
    /// </summary>
    public class BaseDesktopClient<C> : BaseClient<C, IAuthorizationCodeGrantConfig> where C : BaseDesktopClient<C>
    {
        #region Private fields

        /// <summary>
        /// Internal flag tracking if CefSharp initialization has been done.
        /// </summary>
        private static bool cefSharpInitialized;

        /// <summary>
        /// Identifier of this system, used when communicating with the authorization
        /// endpoint of the 10Duke Entitlement service.
        /// </summary>
        private string computerId;

        #endregion

        #region Properties

        /// <summary>
        /// Authorization process result information received from the 10Duke Entitlement service.
        /// </summary>
        public AuthorizationInfo Authorization { get; set; }

        /// <summary>
        /// Configuration specifying how this system is identified when communicating with the authorization
        /// endpoint of the 10Duke Entitlement service. If <c>null</c>, default configuration for computer
        /// identity computing is used.
        /// </summary>
        public ComputerIdentityConfig ComputerIdentityConfig { get; set; }

        /// <summary>
        /// Gets the identifier of this system, used when communicating with the authorization
        /// endpoint of the 10Duke Entitlement service.
        /// </summary>
        public string ComputerId
        {
            get
            {
                var retValue = computerId;
                if (retValue == null)
                {
                    ComputerIdentity.ComputerIdentifier[] idComponents =
                        ComputerIdentityConfig == null || (ComputerIdentityConfig.ComputeBy == null && ComputerIdentityConfig.AdditionalIdentifier == null)
                        ? new[] { ComputerIdentity.ComputerIdentifier.BaseboardSerialNumber } // Uses BaseboardSerialNumber as default
                        : ComputerIdentityConfig.ComputeBy;
                    if (ComputerIdentityConfig == null)
                    {
                        retValue = ComputerIdentity.BuildComputerId(null, null, idComponents);
                    }
                    else if (ComputerIdentityConfig.ComputerId == null)
                    {
                        retValue = ComputerIdentity.BuildComputerId(ComputerIdentityConfig.AdditionalIdentifier, ComputerIdentityConfig.Salt, idComponents);
                    }
                    else
                    {
                        retValue = ComputerIdentityConfig.ComputerId;
                    }

                    computerId = retValue;
                }

                return retValue;
            }
        }

        /// <summary>
        /// OAuth 2.0 access token for accessing APIs that require authorization.
        /// </summary>
        public override string AccessToken
        {
            get
            {
                return Authorization == null || Authorization.AccessTokenResponse == null ? null : Authorization.AccessTokenResponse.AccessToken;
            }

            set
            {
                throw new InvalidOperationException("AccessToken can not be set directly, set Authorization instead");
            }
        }

        /// <summary>
        /// Configuration for communicating with the <c>/authz/</c> API of the 10Duke Entitlement service.
        /// </summary>
        public override IAuthzApiConfig AuthzApiConfig
        {
            get
            {
                return base.AuthzApiConfig ?? Client.Config.AuthzApiConfig.FromOAuthConfig(OAuthConfig);
            }

            set
            {
                base.AuthzApiConfig = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="AuthzApi"/> object for accessing the <c>/authz/</c> API of the 10Duke Identity and Entitlement service.
        /// Please note that the OAuth authentication / authorization process must be successfully completed before
        /// getting the <see cref="AuthzApi"/> object, and the <see cref="AccessToken"/> must be available.
        /// </summary>
        public new AuthzApi AuthzApi
        {
            get
            {
                var retValue = base.AuthzApi;
                retValue.ComputerId = ComputerId;
                return retValue;
            }
        }

        /// <summary>
        /// Gets an <see cref="EntClientAuthorizationSerializer"/> for reading and writing <see cref="Authorization"/>
        /// of this object by binary serialization.
        /// </summary>
        public EntClientAuthorizationSerializer<C> AuthorizationSerializer
        {
            get
            {
                return new EntClientAuthorizationSerializer<C>() { EntClient = (C) this };
            }
        }

        #endregion

        #region Application-wide initialization and clean-up

        /// <summary>
        /// <para>Executes initialization necessary for using <see cref="BaseDesktopClient{C}"/> in an application.
        /// This method (any overload of this method) must be called once during application lifecycle,
        /// before using <see cref="EntClient"/> for the first time.</para>
        /// <para><see cref="EntClient"/> uses <see cref="https://github.com/cefsharp/CefSharp"/> for displaying
        /// an embedded browser window for operations that require displaying server-side user interface,
        /// especially a sign-on window. <see cref="EntClient"/> supports using <c>AnyCPU</c> as target architecture,
        /// and with <c>CefSharp</c> this means that loading unmanaged CefSharp resources is required. This
        /// method assumes that the required CefSharp dependencies can be found under <c>x84</c> or <c>x64</c>
        /// subdirectories.</para>
        /// <para>This overload uses the default settings, using
        /// <see cref="AppDomain.CurrentDomain.SetupInformation.ApplicationBase"/> as the base directory
        /// under which the architecture dependent CefSharp resource subdirectories must be found.</para>
        /// </summary>
        /// <param name="cefSettings">CefSharp initialization parameters. In many cases it is sufficient to
        /// pass an empty instance of a derived class suitable for the use case. Must not be <c>null</c>.</param>
        public static void Initialize(AbstractCefSettings cefSettings)
        {
            Initialize(cefSettings, new CefSharpResolverArgs() { BaseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) });
        }

        /// <summary>
        /// <para>Executes initialization necessary for using <see cref="BaseDesktopClient{C}"/> in an application.
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
        /// pass an empty instance of a derived class suitable for the use case. Must not be <c>null</c>.</param>
        /// <param name="resolverArgs">Arguments for customizing how CefSharp / cef resources are searched,
        /// or <c>null</c> for default behavior.</param>
        public static void Initialize(AbstractCefSettings cefSettings, CefSharpResolverArgs resolverArgs = null)
        {
            CefSharpUtil.InitializeCefSharp(cefSettings, resolverArgs);
            cefSharpInitialized = true;
        }

        /// <summary>
        /// Cleans up resources required for using <see cref="BaseDesktopClient{C}"/> in an application.
        /// This method must be called once when <see cref="BaseDesktopClient{C}"/> is no more used by the application.
        /// </summary>
        public static void Shutdown()
        {
            if (cefSharpInitialized)
            {
                CefSharpUtil.ShutdownCefSharp();
                cefSharpInitialized = false;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Discards authorization information received from the server by setting <see cref="Authorization"/> to <c>null</c>.
        /// </summary>
        public new void ClearAuthorization()
        {
            Authorization = null;
        }

        #endregion
    }
}
