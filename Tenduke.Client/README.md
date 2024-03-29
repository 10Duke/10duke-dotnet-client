# Client library for 10Duke Identity and Entitlement

This is the base client library, other platform specific client libraries are built on this one.
See the repository [README](https://github.com/10Duke/10duke-dotnet-client/blob/master/README.md) for client library listing.

This base client library can also be used directly, instead of the more specific client libraries derived from this one. The main cases for using this base client library are:

- The application using the client library implements OAuth 2.0 authorization flow by itself, thus not needing a library that implements the OAuth flow
- A specific client library for the desired platform is not available

Main features of this base client library are:

- Querying user info
- Checking and consuming licenses
- Checking end-user permissions
- Releasing consumed licenses

## Installation

The client library is available as a [NuGet package](https://www.nuget.org/packages/Tenduke.Client/). An example for installing the client library using NuGet Package Manager:

### Installation with dotnet cli

```sh
dotnet add package Tenduke.Client
```

### Installation with NuGet PackageManager

```powershell
Install-Package Tenduke.Client
```

## Basic usage

Class `Tenduke.Client.EntClient` is the main implementation class that provides access to the APIs. Configuration and features of `Tenduke.Client.WinForms.EntClient` are introduced below.

### Authentication and authorization

User authentication is outside the scope of this client implementation. For being able to call the APIs, an OAuth access token must be used.

### EntClient initialization

An instance of EntClient can be created like this:

```csharp
var entClient = new EntClient()
{
    OAuthConfig = myOAuthConfig,
    AuthzApiConfig = myAuthzApiConfig,
    AccessToken = myAccessToken
};
```

Here, +myOAuthConfig+ is an instance of +Tenduke.Client.Config.OAuthConfig+, for instance:

```csharp
var myOAuthConfig = new OAuthConfig()
{
    UserInfoUri = "https://my-test-idp.10duke.net/userinfo/"
};
```

Here, it is sufficient to initialize only the +UserInfoUri+, and this Uri must point to an actual 10Duke Entitlement service deployment. 

`myAuthzApiConfig` is an instance of `Tenduke.Client.Config.AuthzApiConfig`, for instance:


```csharp
var myAuthzApiConfig = new AuthzApiConfig()
{
    EndpointUri = "https://my-test-idp.10duke.net/authz/",
    SignerKey = [Public key of 10Duke Entitlement service]
};
```

`EndpointUri` points to the authorization endpoint of the actual deployment. `SignerKey` is the RSA public key that is used for verifying signatures of tokens issued by the 10Duke Entitlement service. The following utility is provided for reading an RSA key from string:

```csharp
var publicKey = Tenduke.Client.Util.CryptoUtil.ReadRsaPublicKey(publicKeyAsString);
```

And finally, `myAccessToken` is the access token that, acquired by completing an OAuth flow. Now, the `EntClient` instance is ready to be used.

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

This call returns a `Tenduke.Client.EntApi.Authz.AuthorizationDecision` object that describes an _authorization decision_, returned as a signed JWT token. The `AuthorizationDecision` indicates if a license lease has been granted (and a license seat has been taken), and the client application can rely on the `AuthorizationDecision` until the object expires. Expiration of the object is the same as expiration of the returned JWT token and expiration of the license lease.

#### Release license

```csharp
var releaseResponse = await entClient.AuthzApi.ReleaseLicenseAsync(tokenResponse["jti"], ResponseType.JWT);
```

This call is used for returning a consumed lease (license seat) back to the license pool.

# Links

- [10Duke Enterprise Documentation](https://docs.enterprise.10duke.com)
- [Release Notes](https://github.com/10Duke/10duke-dotnet-client/releases)
