// Resultado de procesar un webhook de Kapso.
namespace Adif.Application.Dtos;

public sealed class WebhookResultado
{
    public bool Autorizado { get; init; } = true;
    public bool Procesado { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public int MensajesEnviados { get; init; }

    public static WebhookResultado Ignorado(string motivo) => new() { Motivo = motivo };
    public static WebhookResultado NoAutorizado() => new() { Autorizado = false, Motivo = "Firma de webhook inválida." };
}
