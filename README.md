# Client libraries for 10Duke Identity and Entitlement {#_client_libraries_for_10duke_identity_and_entitlement}

Client libraries for .NET applications, for using services of the 10Duke
Identity and Entitlement. Main features are:

-   Querying user info
-   Checking and consuming licenses
-   Releasing consumed licenses
-   Checking end-user permissions
-   Authentication and authorization with OAuth 2.0 and OpenID Connect

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

## Installation {#_installation}

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
