namespace Tenduke.Client.Config
{
    /// <summary>
    /// Wraps another <see cref="IAuthorizationCodeGrantConfig"/> and allows selective overriding.
    /// </summary>
    public class AuthorizationCodeGrantConfigWrapper : BrowserBasedAuthorizationConfigWrapper, IAuthorizationCodeGrantConfig
    {
        /// <summary>
        /// The wrapped config object.
        /// </summary>
        protected new IAuthorizationCodeGrantConfig Wrapped
        {
            get
            {
                return base.Wrapped as IAuthorizationCodeGrantConfig;
            }
            set
            {
                base.Wrapped = value;
            }
        }

        /// <summary>
        /// OAuth 2.0 client secret. Please note that this is ignored if
        /// <see cref="UsePkce"/> is <c>true</c>.
        /// </summary>
        public string ClientSecret
        {
            get
            {
                return AssertWrapped().ClientSecret;
            }
        }

        /// <summary>
        /// Uri of the OAuth 2.0 token endpoint of the 10Duke Entitlement service.
        /// </summary>
        public string TokenUri
        {
            get
            {
                return AssertWrapped().TokenUri;
            }
        }

        /// <summary>
        /// Indicates if PKCE (Proof Key for Code Exchange) is used. Please note that
        /// if <c>true</c>, <see cref="ClientSecret"/> is ignored.
        /// </summary>
        public bool UsePkce
        {
            get
            {
                return AssertWrapped().UsePkce;
            }
        }

        /// <summary>
        /// Initialized a new instance of the <see cref="AuthorizationCodeGrantConfigWrapper"/> class.
        /// </summary>
        /// <param name="wrapped">The wrapped config object.</param>
        protected AuthorizationCodeGrantConfigWrapper(IAuthorizationCodeGrantConfig wrapped)
            : base(wrapped)
        {
        }

        /// <summary>
        /// Returns value of <see cref="Wrapped"/>, asserting that it is not <c>null</c>.
        /// </summary>
        /// <returns>The wrapped <see cref="IAuthorizationCodeGrantConfig"/> object.</returns>
        protected new IAuthorizationCodeGrantConfig AssertWrapped()
        {
            return base.AssertWrapped() as IAuthorizationCodeGrantConfig;
        }
    }
}
