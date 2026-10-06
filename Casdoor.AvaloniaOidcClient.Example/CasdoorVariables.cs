using Casdoor.Client;
using System.Net.Http;

namespace Casdoor.AvaloniaOidcClient.Example
{
    // The Casdoor application to sign in with, the defaults are the public demo server https://door.casdoor.com
    internal static class CasdoorVariables
    {
        public const string Domain = "https://door.casdoor.com";
        public const string AppName = "app-casnode";
        public const string OrganizationName = "casbin";
        public const string ClientId = "014ae4bd048734ca2dea";
        // Casdoor redirects here after signing in, the app catches it in the WebView instead of opening it.
        // Must be in the Redirect URLs of the application.
        public const string CallbackUrl = "casdoor://callback";

        // No client secret: a desktop app can't keep one, the authorization code is protected by PKCE instead
        public static readonly CasdoorOptions Options = new CasdoorOptions
        {
            Endpoint = Domain,
            OrganizationName = OrganizationName,
            ApplicationName = AppName,
            ApplicationType = "native",
            ClientId = ClientId,
            CallbackPath = CallbackUrl,
            RequireHttpsMetadata = false,
            Scope = "profile email"
        };

        public static readonly CasdoorClient Client = new CasdoorClient(new HttpClient(), Options);
    }
}
