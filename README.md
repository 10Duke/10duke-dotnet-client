# THIS REPOSITORY IS UNDER LIMITED MAINTENANCE

We've introduced a new generation of libraries for .NET, the [10Duke Enterprise SDK for .NET](https://gitlab.com/10Duke/enterprise/dotnet/dotnet-enterprise-sdk),
to facilitate building applications that are licensed using [10Duke Enterprise](https://docs.enterprise.10duke.com/).
We recommend new customers or new applications being integrated with [10Duke Enterprise](https://docs.enterprise.10duke.com/)
use this latest generation of libraries.

This library and the associated Nuget packages will receive security updates and limited maintenance
until further notice.

New feature development will be limited to the
[10Duke Enterprise SDK for .NET](https://gitlab.com/10Duke/enterprise/dotnet/dotnet-enterprise-sdk).

# Client libraries for 10Duke Identity and Entitlement

Client libraries for .NET applications, for using services of the 10Duke
Identity and Entitlement. Main features are:

- Authentication and authorization with OAuth 2.0 and OpenID Connect
- Querying user info
- Checking and consuming licenses
- Checking end-user permissions
- Releasing consumed licenses

| Package | Application type | .NET target framework(s) |
| --- | --- | --- |
| [Tenduke.Client.DefaultBrowser](https://github.com/10Duke/10duke-dotnet-client/tree/master/Tenduke.Client.DefaultBrowser) | Windows Desktop / Any application running on an OS that can launch the OS default browser | .NET Standard 2.0 (netstandard2.0)    |
| [Tenduke.Client.WPF](https://github.com/10Duke/10duke-dotnet-client/tree/master/Tenduke.Client.WPF) | Windows Desktop / WPF | .NET 8.0 (net8.0-windows) / .NET Framework 4.6.2 (net462) | 
| [Tenduke.Client.WinForms](https://github.com/10Duke/10duke-dotnet-client/tree/master/Tenduke.Client.WinForms) | Windows Desktop / WinForms | .NET 8.0 (net8.0-windows) / .NET Framework 4.6.2 (net462) |
| [Tenduke.Client.AspNetCore](https://github.com/10Duke/10duke-dotnet-client/tree/master/Tenduke.Client.AspNetCore) | Web / ASP.NET Core | .NET Standard 2.0 (netstandard2.0) | 

## Client libraries

All the client libraries are based on the generic
[Tenduke.Client](https://github.com/10Duke/10duke-dotnet-client/tree/master/Tenduke.Client)
base library, which can be used as a basis for building a client for any
.NET Standard 2.0 compatible platform.

## Installation

All the client libraries are distributed via NuGet and can be installed via dotnet cli, Nuget command line,
or the nuget package manager user interface in Visual Studio.

For example of the Default Browser client library can be installed as follows:

### Installation with NuGet PackageManager

```powershell
Install-Package Tenduke.Client.DefaultBrowser
```

### Installation with dotnet cli

```sh
dotnet add package Tenduke.Client.DefaultBrowser
```
