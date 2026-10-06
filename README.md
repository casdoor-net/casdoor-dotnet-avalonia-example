# Casdoor .NET Avalonia Example

[![Build](https://github.com/casdoor-net/casdoor-dotnet-avalonia-example/actions/workflows/build.yml/badge.svg)](https://github.com/casdoor-net/casdoor-dotnet-avalonia-example/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/casdoor-net/casdoor-dotnet-avalonia-example)](https://github.com/casdoor-net/casdoor-dotnet-avalonia-example/blob/master/LICENSE)
[![Discord](https://img.shields.io/discord/1022748306096537660?logo=discord&label=discord&color=5865F2)](https://discord.gg/5rPsrAzK7S)

An example cross-platform desktop app, built with [Avalonia](https://avaloniaui.net/), that signs users in with [Casdoor](https://casdoor.ai/) using [casdoor-dotnet-sdk](https://github.com/casdoor-net/casdoor-dotnet-sdk) and the OAuth 2.0 authorization code flow with PKCE.

<img src="images/casdoor-signin.gif" alt="sign in" height="600"/>

## How it works

1. **Sign in** shows the Casdoor sign-in page in a [WebView](https://github.com/MicroSugarDeveloperOrg/Avalonia.WebView). The URL comes from `CasdoorClient.GetSigninUrl()` and carries a PKCE code challenge and a random `state` ([AccountView.axaml.cs](Casdoor.AvaloniaOidcClient.Example/Views/AccountView.axaml.cs)).
2. After signing in, Casdoor redirects to the callback URL `casdoor://callback?code=...&state=...`. The app catches that navigation in the WebView, checks the state and takes the code.
3. The app exchanges the code for the tokens with the PKCE code verifier (`RequestAuthorizationCodeTokenAsync()`), so no client secret is stored in the app.
4. It reads the user with the access token (`UserInfo()`) and shows the name and email. **Log out** ends the Casdoor session of the WebView.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or newer
- On Windows, the [WebView2 Runtime](https://developer.microsoft.com/microsoft-edge/webview2/#download-section), preinstalled on Windows 10 and 11
- A Casdoor server. The example is preconfigured for the public demo server https://door.casdoor.com, so it runs as is. To use your own, see [Casdoor installation](https://casdoor.ai/docs/basic/server-installation).

## Configuration

Skip this section to try the example with the public demo server.

In your Casdoor, create (or reuse) an organization and an application, and add `casdoor://callback` to the application's **Redirect URLs**. Then fill in [CasdoorVariables.cs](Casdoor.AvaloniaOidcClient.Example/CasdoorVariables.cs):

| Name             | Description                                                   |
|------------------|---------------------------------------------------------------|
| Domain           | Casdoor server URL                                            |
| ClientId         | Client ID of the application                                  |
| AppName          | Name of the application                                       |
| OrganizationName | Organization of the application                               |
| CallbackUrl      | Callback URL, must be in the Redirect URLs of the application |

## Run

```shell
git clone https://github.com/casdoor-net/casdoor-dotnet-avalonia-example
cd casdoor-dotnet-avalonia-example
dotnet run --project Casdoor.AvaloniaOidcClient.Example
```

Click **Sign in**. On the demo server, sign in with username `admin` and password `123`.

## Resources

- [Casdoor documentation](https://casdoor.ai/docs/overview)
- [casdoor-dotnet-sdk](https://github.com/casdoor-net/casdoor-dotnet-sdk)
- [casdoor-dotnet-desktop-example](https://github.com/casdoor-net/casdoor-dotnet-desktop-example) (WPF) and [casdoor-dotnet-winform-example](https://github.com/casdoor-net/casdoor-dotnet-winform-example) (Windows Forms)

## License

[Apache-2.0](LICENSE)
