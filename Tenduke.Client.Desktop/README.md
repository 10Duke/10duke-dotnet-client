# Tenduke Client Library for Windows Desktop Applications

This library is for use with the Tenduke.Client package to provide Windows desktop
applications using either WinForms or WPF with access to services of the 10Duke
Identity and Entitlement.

Main features are:

- Authentication and authorization with OAuth 2.0 and OpenID Connect
- Querying user info
- Checking and consuming licenses
- Releasing consumed licenses
- Checking end-user permissions

Provides a base client for use in desktop applications (either WinForms or WPF).

This library is intended to be installed as a dependency of either Tenduke.Client.WinForms
or Tenduke.Client.WPF.

## Installation with dotnet cli

```sh
dotnet add package Tenduke.Client.Desktop
```

## Installation with NuGet PackageManager

```powershell
Install-Package Tenduke.Client.Desktop
```
