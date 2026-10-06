using System;

namespace Casdoor.AvaloniaOidcClient.Example;

public class CodeReceivedEventArgs : EventArgs
{
    public CodeReceivedEventArgs(string code, string codeVerifier)
    {
        Code = code;
        CodeVerifier = codeVerifier;
    }

    public string Code { get; }
    public string CodeVerifier { get; }
}
