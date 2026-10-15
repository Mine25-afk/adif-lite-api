using System.Text;
using Adif.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers.V1;

// Receptor de webhooks de Kapso; la autenticación es la firma HMAC por número.
[ApiController]
[AllowAnonymous]
[Route(ApiRoutes.V1 + "/whatsapp")]
public sealed class WhatsappWebhookController(IWebhookKapsoService webhook) : ControllerBase
{
    [HttpPost("webhook")]
    [RequestSizeLimit(256 * 1024)]
    public async Task<IActionResult> Recibir(CancellationToken ct)
    {
        // La firma se calcula sobre los bytes exactos: se lee el cuerpo crudo, sin model binding.
        using var lector = new StreamReader(Request.Body, Encoding.UTF8);
        var cuerpo = await lector.ReadToEndAsync(ct);

        var resultado = await webhook.ProcesarAsync(cuerpo, Request.Headers["X-Webhook-Signature"].FirstOrDefault(), Request.Headers["X-Webhook-Event"].FirstOrDefault(), ct);
        if (!resultado.Autorizado) return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "No autorizado", detail: resultado.Motivo);

        return Ok(new { ok = true, procesado = resultado.Procesado, motivo = resultado.Motivo, enviados = resultado.MensajesEnviados });
    }
}
