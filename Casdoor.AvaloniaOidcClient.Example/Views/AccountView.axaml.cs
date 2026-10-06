using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Security.Cryptography;
using System.Web;
using WebViewCore.Enums;
using WebViewCore.Events;

namespace Casdoor.AvaloniaOidcClient.Example.Views;

/// <summary>
/// Shows the Casdoor sign-in page in a WebView, catches the redirect to the callback URL
/// and exchanges the code for the tokens with PKCE.
/// </summary>
public partial class AccountView : UserControl
{
    // PKCE: only the app that started the sign-in knows the verifier, so a stolen code is useless
    private string _codeVerifier = "";
    // the state ties the callback to this sign-in
    private string _state = "";
    private bool _signingIn;

    public AccountView()
    {
        InitializeComponent();
        Loaded += AccountView_Loaded;
        CodeReceived += AccountView_CodeReceived;
    }

    public event EventHandler<CodeReceivedEventArgs>? CodeReceived;

    private static string RandomString()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private void AccountView_Loaded(object? sender, RoutedEventArgs e)
    {
        // Casdoor goes to the callback URL either in the page or in a new window
        PART_WebView.NavigationStarting += PART_WebView_NavigationStarting;
        PART_WebView.WebViewNewWindowRequested += PART_WebView_WebViewNewWindowRequested;
    }

    private void LoginBtn_Click(object sender, RoutedEventArgs e)
    {
        _codeVerifier = RandomString();
        _state = RandomString();
        _signingIn = true;

        var loginUrl = CasdoorVariables.Client.GetSigninUrl(_codeVerifier, true)
            .Replace($"&state={CasdoorVariables.AppName}", $"&state={_state}");

        ShowPanel(WebPanel);
        PART_WebView.Url = new Uri(loginUrl);
    }

    private void LogoutBtn_Click(object sender, RoutedEventArgs e)
    {
        // ends the Casdoor session of the WebView, so that signing in again asks for the password
        PART_WebView.Url = new Uri($"{CasdoorVariables.Domain}/api/logout");
        ShowPanel(StartPanel);
    }

    private void PART_WebView_NavigationStarting(object? sender, WebViewUrlLoadingEventArg e)
    {
        if (e.Url is not null && IsCallback(e.Url))
        {
            e.Cancel = true;
            HandleCallback(e.Url);
        }
    }

    private void PART_WebView_WebViewNewWindowRequested(object? sender, WebViewNewWindowEventArgs e)
    {
        if (e.Url is not null && IsCallback(e.Url))
        {
            e.UrlLoadingStrategy = UrlRequestStrategy.CancelLoad;
            HandleCallback(e.Url);
        }
    }

    private static bool IsCallback(Uri url)
    {
        return url.AbsoluteUri.StartsWith(CasdoorVariables.CallbackUrl, StringComparison.OrdinalIgnoreCase);
    }

    private void HandleCallback(Uri url)
    {
        if (!_signingIn)
        {
            return;
        }
        _signingIn = false;

        var query = HttpUtility.ParseQueryString(url.Query);
        var code = query.Get("code");
        var error = query.Get("error");
        if (!string.IsNullOrEmpty(error))
        {
            ShowError($"Failed to sign in: {error} {query.Get("error_description")}");
        }
        else if (query.Get("state") != _state || string.IsNullOrEmpty(code))
        {
            ShowError("Failed to sign in: invalid state or code, please try again.");
        }
        else
        {
            var args = new CodeReceivedEventArgs(code, _codeVerifier);
            Dispatcher.UIThread.Post(() => CodeReceived?.Invoke(this, args));
        }
    }

    private async void AccountView_CodeReceived(object? sender, CodeReceivedEventArgs e)
    {
        ShowPanel(StartPanel);
        MessageText.Text = "Loading...";

        try
        {
            // exchange the code for the tokens, with the PKCE code verifier instead of a client secret
            var token = await CasdoorVariables.Client.RequestAuthorizationCodeTokenAsync(
                e.Code, CasdoorVariables.CallbackUrl, e.CodeVerifier);
            if (token.IsError || string.IsNullOrEmpty(token.AccessToken))
            {
                throw new InvalidOperationException(token.Error ?? "no access token");
            }

            var user = await CasdoorVariables.Client.UserInfo(token.AccessToken);
            UsernameLabel.Content = user?.Name;
            EmailLabel.Content = user?.Email;
            MessageText.Text = "";
            ShowPanel(AccountPanel);
        }
        catch (Exception ex)
        {
            ShowError($"Failed to sign in: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ShowPanel(StartPanel);
            MessageText.Text = message;
        });
    }

    private void ShowPanel(Control panel)
    {
        StartPanel.IsVisible = panel == StartPanel;
        AccountPanel.IsVisible = panel == AccountPanel;
        WebPanel.IsVisible = panel == WebPanel;
    }
}
