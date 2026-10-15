using Adif.Application.Abstractions.Services;
using Adif.Application.Dtos;
using Microsoft.Extensions.Logging;

namespace Adif.Application.Services;

public sealed class WebhookKapsoService(
    IWhatsappContextService contexto,
    IFiadoBotService bot,
    IDeduplicadorMensajes deduplicador,
    ILogger<WebhookKapsoService> logger) : IWebhookKapsoService
{
    public async Task<WebhookResultado> ProcesarAsync(string cuerpo, string? firma, string? eventoHeader, CancellationToken ct = default)
    {
        var (mensaje, motivo) = KapsoWebhookParser.Parsear(cuerpo, eventoHeader);
        if (mensaje is null) return WebhookResultado.Ignorado(motivo);

        // Cada cliente firma con su propio secreto: un número desconocido no se puede verificar.
        var numero = await contexto.ObtenerNumeroAsync(mensaje.PhoneNumberId, ct);
        if (numero is null || !FirmaWebhook.EsValida(cuerpo, numero.WebhookSecret, firma))
        {
            logger.LogWarning("Webhook rechazado para phone_number_id {PhoneNumberId}: firma inválida o número no registrado", mensaje.PhoneNumberId);
            return WebhookResultado.NoAutorizado();
        }

        if (mensaje.Saliente) return WebhookResultado.Ignorado("Mensaje saliente.");
        if (mensaje.From.Length == 0) return WebhookResultado.Ignorado("Mensaje sin remitente.");
        if (mensaje.Id.Length > 0 && !deduplicador.EsNuevo(mensaje.Id)) return WebhookResultado.Ignorado("Mensaje duplicado.");

        try
        {
            var enviados = await bot.ProcesarAsync(new NegocioDelMensaje(numero.NegocioId, new CuentaWhatsapp(numero.PhoneNumberId, numero.KapsoApiKey)), mensaje, ct);
            return new WebhookResultado { Procesado = true, MensajesEnviados = enviados.Count };
        }
        catch (Exception ex)
        {
            // Se responde 200 igual: un 5xx hace que Kapso reintente y duplique operaciones.
            logger.LogError(ex, "Error procesando el mensaje {MensajeId} del negocio {NegocioId}", mensaje.Id, numero.NegocioId);
            return WebhookResultado.Ignorado("Error interno al procesar el mensaje.");
        }
    }
}
