using Adif.Application.Dtos;

namespace Adif.Application.Services;

public interface IWebhookKapsoService
{
    Task<WebhookResultado> ProcesarAsync(string cuerpo, string? firma, string? eventoHeader, CancellationToken ct = default);
}
