# WinForms client library for 10Duke Identity and Entitlement

Client library for .NET Windows Forms (WinForms) applications, for using
services of the 10Duke Identity and Entitlement. Main features are:

- Authentication and authorization with OAuth 2.0 and OpenID Connect
- Querying user info
- Checking and consuming licenses
- Checking end-user permissions
- Releasing consumed licenses

## Installation

The client library is available as a [NuGet
package](https://www.nuget.org/packages/Tenduke.Client.WinForms/). An
example for installing the client library using NuGet Package Manager:

### Installation with dotnet cli

```sh
dotnet add package Tenduke.Client.WinForms
```

### Installation with NuGet PackageManager

```powershell
Install-Package Tenduke.Client.WinForms
```

## Basic usage

Class `Tenduke.Client.WinForms.EntClient` is the main implementation
class that supports integrating a WinForms application to 10Duke
Identity and Entitlement service. Configuration and features of
`Tenduke.Client.WinForms.EntClient` are introduced below.

### Authentication and authorization

When using 10Duke Identity, the client application delegates
authentication to the 10Duke Identity Provider. For using 10Duke APIs,
including Entitlement, the client application must authorize the API
calls by presenting an OAuth 2.0 access token. The simplest way to
achieve both authentication (and Single Sign-On) and authorization is to
use OpenID Connect (OIDC). OpenID Connect is based on OAuth 2.0, and as
a result of the sign-on flow both identity and authorization are
established.

Authentication and authorization are implemented using [OpenID Connect
Authorization Code Grant
flow](https://openid.net/specs/openid-connect-core-1_0.html#CodeFlowAuth)
with [PKCE](https://tools.ietf.org/html/rfc7636). An embedded browser is
used for user interaction, most notably for the login prompt. This
approach is secure, flexible and unleashes advanced use cases like
federated authentication.

### Embedded browser

The client library uses the [CEFSharp](https://cefsharp.github.io/)
library that is based on [Chromium Embedded
Framework](https://bitbucket.org/chromiumembedded/cef). All the required
components are bundled with the client library. CEFSharp must be
initialized when the client application starts, before the browser
window is opened for the first time, with the following method call:

```csharp
var resolverArgs = Tenduke.Client.Desktop.Util.CefSharpUtil.AddAssemblyResolverForCefSharp();
```

This initialization call is a static call and must be called once in the
lifecycle of the client application. The returned `resolverArgs` object
is needed later for initializing the
`Tenduke.Client.WinForms.EntClient`.

#### Customizing the browser form

The browser form showing the embedded browser can be customized by
handling the `RaiseInitializeBrowserForm` event and setting form
properties in the event handler. This example changes the form title:

```csharp
...
entClient.RaiseInitializeBrowserForm += HandleInitializeBrowserForm;
...

private void HandleInitializeBrowserForm(object sender, InitializeBrowserFormEventArgs e)
{
        e.WebBrowserForm.Text = "Acme login";
}
```

### EntClient initialization and clean-up

After initializing the CEFSharp embedded browser component, the
`Tenduke.Client.WinForms.EntClient` can be initialized by calling:

```csharp
Tenduke.Client.WinForms.EntClient.Initialize(resolverArgs);
```

Here, `resolverArgs` is the object returned by the CEFSharp
initialization call described above. Also this initialization call is a
static method call that must be done once in the client application
lifecycle.

After static initialization an instance of EntClient can be created:

```csharp
var entClient = new EntClient() { OAuthConfig = myOAuthConfig };
```

Here, `myOAuthConfig` is an instance of
`Tenduke.Client.Config.AuthorizationCodeGrantConfig`, for instance:

```csharp
var myOAuthConfig = new AuthorizationCodeGrantConfig()
{
    AuthzUri = "https://my-test-idp.10duke.net/user/oauth20/authz",
    TokenUri = "https://my-test-idp.10duke.net/user/oauth20/token",
    UserInfoUri = "https://my-test-idp.10duke.net/user/info",
    ClientID = "my-client-id",
    ClientSecret = null,
    RedirectUri = "oob:MyTestApplication",
    Scope = "openid profile email",
    SignerKey = [Public key of 10Duke Entitlement service],
    ShowRememberMe = true,
    UsePkce = true,
    AllowInsecureCerts = false
};
```

Here, the Uris must point to an actual 10Duke Entitlement service
deployment. `ClientID`, `ClientSecret` (only used if `UsePkce` is
`false`) and `RedirectUri` are standard OAuth parameters and they must
match values configured in the 10Duke Entitlement service deployment.
`Scope` is the OAuth / OpenID Connect scope required by the client
application, usually at least the `openid` and `profile` scope should be
specified. `SignerKey` is the RSA public key that is used for verifying
signatures of tokens issued by the 10Duke Entitlement service. The
following utility is provided for reading an RSA key from string, where
the string can be either an RSA public key in PEM format or URL of
server JWKS endpoint:

```csharp
var publicKey = await Tenduke.Client.Util.CryptoUtil.ReadFirstRsaPublicKey(publicKeyOrJwksUrl, new HttpClient());
```

Now, the `EntClient` instance is ready to be used for user
authentication, license requests etc.

When the client application is closing, the following call must be
executed to shut down `EntClient` and clean up resources:

```csharp
Tenduke.Client.WinForms.EntClient.Shutdown();
```

This is a static method call to be called once in the client application
lifecycle.

### User authentication and OAuth authorization

User authentication is started with this call:

```csharp
entClient.AuthorizeSync();
```

This starts the OAuth 2.0 / OpenID Connect flow and opens a modal window
with an embedded browser. User logs in in this window. What happens next
is fully handled by the client library: Internally a standard OAuth
redirect is executed to the specified redirect URI. Example redirect URI
`oob:MyTestApplication` is used above, but any URI with a custom schema
can be used. By convention, the `oob:` (Out Of Band) is used in most
cases.

When login is completed the modal window closes and the `EntClient`
instance holds the login state. The following call tells if the login
has been completed successfully:

```csharp
var success = entClient.IsAuthorized();
```

Full OAuth authorization data included OpenID Connect ID Token is stored
in the `Authorization` property:

```csharp
var authorizationInfo = entClient.Authorization;
```

### Using the client to make 10Duke API calls

Example user info and license requests are given below:

#### User info request

```csharp
var userInfo = await entClient.UserInfoApi.GetUserInfoAsync();
```

This call returns an object with OpenID Connect user info.

#### Consume license

```csharp
    var tokenResponse = await entClient.AuthzApi.CheckOrConsumeAsync("MyLicense", true, ResponseType.JWT);
```

The call above returns a
`Tenduke.Client.EntApi.Authz.AuthorizationDecision` object that
describes an *authorization decision*, returned as a signed JWT token.
The `AuthorizationDecision` indicates if a license lease has been
granted (and a license seat has been taken), and the client application
can rely on the `AuthorizationDecision` until the object expires.
Expiration of the object is the same as expiration of the returned JWT
token and expiration of the license lease.

```csharp
var tokenResponse = await entClient.AuthzApi.CheckOrConsumeAsync(
    "MyLicense",
    true,
    ResponseType.JWT,
    ConsumptionMode.Cache,
    new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>("licenseId", licenseId) });
```

This example specifies some more parameters to the consumption request.
The last parameter shown in the example can be used for giving any
additional claims understood by the license consumption endpoint.
Standard additional claims include `licenseId` and `entitlementId` that
can be used for explicitly selecting the license or entitlement to
consume. In basic use cases for consuming if a valid license is found,
these parameters are not required.

License consumption requests compute a computer id that is sent with the
consumption requests in order to identify the client hardware. Computer
id can be customized by setting `ComputerIdentityConfig`, for example
the following configuration makes computer id computation use
FIPS-compliant SHA256 hash algorithm:

```csharp
entClient.ComputerIdentityConfig = new ComputerIdentityConfig() { HashAlg = Desktop.Util.ComputerIdentity.HashAlg.SHA256 };
```

#### Release license

```csharp
var tokenResponse = await entClient.AuthzApi.ReleaseLicenseAsync(tokenResponse["jti"], ResponseType.JWT);
```

This call is used for returning a consumed lease (license seat) back to
the license pool.

# Links

- [10Duke Enterprise Documentation](https://docs.enterprise.10duke.com)
- [Release Notes](https://github.com/10Duke/10duke-dotnet-client/releases)
