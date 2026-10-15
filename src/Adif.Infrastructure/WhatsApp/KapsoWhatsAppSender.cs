using System.Net.Http.Json;
using Adif.Application.Abstractions.Services;
using Adif.Application.Dtos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Adif.Infrastructure.WhatsApp;

// Sin API key (ni del número ni global) el mensaje solo se simula: sirve para probar el bot sin gastar envíos.
public sealed class KapsoWhatsAppSender(HttpClient http, IOptions<KapsoOptions> opciones, ILogger<KapsoWhatsAppSender> logger) : IWhatsAppSender
{
    public Task<EnvioWhatsapp> EnviarTextoAsync(CuentaWhatsapp cuenta, string para, string texto, CancellationToken ct = default) => Enviar(cuenta, "text", texto, KapsoPayloads.Texto(para, texto), ct);

    public Task<EnvioWhatsapp> EnviarBotonesAsync(CuentaWhatsapp cuenta, string para, string texto, IReadOnlyList<BotonWhatsapp> botones, CancellationToken ct = default) => Enviar(cuenta, "button", texto, KapsoPayloads.Botones(para, texto, botones), ct);

    public Task<EnvioWhatsapp> EnviarListaAsync(CuentaWhatsapp cuenta, string para, string texto, string textoBoton, IReadOnlyList<OpcionListaWhatsapp> opciones, CancellationToken ct = default) => Enviar(cuenta, "list", texto, KapsoPayloads.Lista(para, texto, textoBoton, opciones), ct);

    private async Task<EnvioWhatsapp> Enviar(CuentaWhatsapp cuenta, string tipo, string texto, object payload, CancellationToken ct)
    {
        var apiKey = string.IsNullOrWhiteSpace(cuenta.ApiKey) ? opciones.Value.ProjectApiKey : cuenta.ApiKey;
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogInformation("Kapso sin API key para {PhoneNumberId}: mensaje {Tipo} simulado", cuenta.PhoneNumberId, tipo);
            return new EnvioWhatsapp { Simulado = true, Tipo = tipo, Texto = texto };
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{opciones.Value.BaseUrl.TrimEnd('/')}/{cuenta.PhoneNumberId}/messages") { Content = JsonContent.Create(payload) };
            request.Headers.Add("X-API-Key", apiKey);

            using var response = await http.SendAsync(request, ct);
            if (response.IsSuccessStatusCode) return new EnvioWhatsapp { Enviado = true, Tipo = tipo, Texto = texto };

            var cuerpo = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning("Kapso rechazó el envío ({StatusCode}) para {PhoneNumberId}: {Respuesta}", (int)response.StatusCode, cuenta.PhoneNumberId, KapsoPayloads.Recortar(cuerpo, 500));
            return new EnvioWhatsapp { Tipo = tipo, Texto = texto, Error = $"HTTP {(int)response.StatusCode}" };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            logger.LogWarning(ex, "Falló el envío a Kapso para {PhoneNumberId}", cuenta.PhoneNumberId);
            return new EnvioWhatsapp { Tipo = tipo, Texto = texto, Error = ex.Message };
        }
    }
}
