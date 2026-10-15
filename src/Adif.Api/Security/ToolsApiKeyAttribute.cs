using System.Security.Cryptography;
using System.Text;
using Adif.Infrastructure.WhatsApp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Adif.Api.Security;

// Exige X-API-Key; sin clave configurada RECHAZA todo (falla cerrado, nunca abierto).
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ToolsApiKeyAttribute : Attribute, IAuthorizationFilter
{
    public const string Header = "X-API-Key";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var esperada = context.HttpContext.RequestServices.GetRequiredService<IOptions<KapsoOptions>>().Value.ToolsApiKey;
        var recibida = context.HttpContext.Request.Headers[Header].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(esperada) || string.IsNullOrEmpty(recibida) || !SonIguales(esperada, recibida))
            context.Result = new ObjectResult(new ProblemDetails { Status = StatusCodes.Status401Unauthorized, Title = "No autorizado", Detail = "API key inválida o ausente." }) { StatusCode = StatusCodes.Status401Unauthorized };
    }

    private static bool SonIguales(string a, string b) => CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(a)), SHA256.HashData(Encoding.UTF8.GetBytes(b)));
}
