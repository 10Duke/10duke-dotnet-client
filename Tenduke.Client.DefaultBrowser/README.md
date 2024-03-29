# Desktop / generic client library for 10Duke Identity and Entitlement

Client library for .NET applications that can access the default browser
of the operating system, for using services of the 10Duke Identity and
Entitlement.

Main features are:

- Authentication and authorization with OAuth 2.0 and OpenID Connect
- Querying user info
- Checking and consuming licenses
- Checking end-user permissions
- Releasing consumed licenses

## Installation

The client library is available as a [NuGet
package](https://www.nuget.org/packages/Tenduke.Client.DefaultBrowser/).
An example for installing the client library using NuGet Package
Manager:

### Installation with dotnet cli

```sh
dotnet add package Tenduke.Client.DefaultBrowser
```

### Installation with NuGet PackageManager

```powershell
Install-Package Tenduke.Client.DefaultBrowser
```

## Basic usage

Class `Tenduke.Client.DefaultBrowser.EntClient` is the main
implementation class that supports integrating a .NET application to
10Duke Identity and Entitlement service. Configuration and features of
`Tenduke.Client.DefaultBrowser.EntClient` are introduced below.

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
with [PKCE](https://tools.ietf.org/html/rfc7636). Registered default
browser of the operating system is used for user interaction, most
notably for the login prompt. This approach is secure, flexible and
unleashes advanced use cases like federated authentication.

### Using the operating system default browser

This client library calls the operating system for running the sign-on
process in the OS default browser. The initial URL opened in the OS
default browser starts the OpenID connect (OIDC) authentication process.
The user is expected to log in to 10Duke Identity and Entitlement (if
not already logged in) in this browser.

When starting the sign-on process, the client library starts an HTTP
listener that waits for the OIDC response. This listener is started in
the first available port in range 49215 - 65535 (IANA suggested range
for dynamic or private ports). The listener will be connected only by
the locally running OS default browser using the local loopback address,
for example `http://127.0.0.1:49215/`.

When user has completed the login process, the browser redirects back to
the client. This sends a request to the HTTP listener opened by the
client library. The client library handles the request, completes the
OIDC process and closes the HTTP listener.

When configuring the client in 10Duke, the local loopback URLs in the
port range described above must be allowed as login callback URLs.

### EntClient initialization

An instance of EntClient can be created like this:

```csharp
var entClient = new EntClient() { OAuthConfig = myOAuthConfig };
```

Here, `myOAuthConfig` is an instance of
`Tenduke.Client.Config.DefaultBrowserAuthorizationCodeGrantConfig`, for
instance:

```csharp
var myOAuthConfig = new DefaultBrowserAuthorizationCodeGrantConfig()
{
    AuthzUri = "https://my-test-idp.10duke.net/user/oauth20/authz",
    TokenUri = "https://my-test-idp.10duke.net/user/oauth20/token",
    UserInfoUri = "https://my-test-idp.10duke.net/user/info",
    ClientID = "my-client-id",
    ClientSecret = null,
    RedirectUri = null,
    Scope = "openid profile email",
    SignerKey = [Public key of 10Duke Entitlement service],
    ShowRememberMe = true,
    UsePkce = true,
    AllowInsecureCerts = false,
    ResponseTimeout = 120
};
```

Here, the Uris must point to an actual 10Duke Entitlement service
deployment. `ClientID`, `ClientSecret` (only used if `UsePkce` is
`false`) and `RedirectUri` are standard OAuth parameters and they must
match values configured in the 10Duke Entitlement service deployment.
`Scope` is the OAuth / OpenID Connect scope required by the client
application, usually at least the `openid` and `profile` scope should be
specified.

`SignerKey` is the RSA public key that is used for verifying signatures
of tokens issued by the 10Duke Entitlement service. The following
utility is provided for reading an RSA key from string, where the string
can be either an RSA public key in PEM format or URL of server JWKS
endpoint:

```csharp
var publicKey = await Tenduke.Client.Util.CryptoUtil.ReadFirstRsaPublicKey(publicKeyOrJwksUrl, new HttpClient());
```

Please note that `RedirectUri` is ignored by this OS default browser
based client library, and a dynamically created HTTP listener is used
instead as described above.

Now, the `EntClient` instance is ready to be used for user
authentication, license requests etc.

### User authentication and OAuth authorization

User authentication is started with this call:

```csharp
entClient.AuthorizeSync();
```

This starts the OAuth 2.0 / OpenID Connect flow in the OS default
browser. User logs in in the browser. The client library waits for the
browser to redirect back to the client, to the HTTP listener started by
the client.

When login is completed, user may close the browser window, and the
`EntClient` instance holds the login state. The following call tells if
the login has been completed successfully:

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
