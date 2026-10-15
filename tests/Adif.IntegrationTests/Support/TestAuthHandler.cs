using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Adif.IntegrationTests.Support;

// Sustituye a Firebase en pruebas: "Authorization: Test <uid>" autentica a ese uid; sin header, 401.
public sealed class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string Esquema = "Test";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith(Esquema + " ", StringComparison.Ordinal)) return Task.FromResult(AuthenticateResult.NoResult());

        var uid = header[(Esquema.Length + 1)..].Trim();
        var identidad = new ClaimsIdentity([new Claim("user_id", uid), new Claim("sub", uid)], Esquema);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identidad), Esquema)));
    }
}
