// Mensaje entrante ya extraído del webhook de Kapso.
namespace Adif.Application.Dtos;

public sealed class MensajeEntrante
{
    public string Id { get; init; } = string.Empty;
    public string PhoneNumberId { get; init; } = string.Empty;
    public string From { get; init; } = string.Empty;
    public string Texto { get; init; } = string.Empty;
    public string RespuestaId { get; init; } = string.Empty;
    public bool Saliente { get; init; }
}
