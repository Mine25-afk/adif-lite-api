// Resultado de un envío a WhatsApp (o de su simulación sin API key).
namespace Adif.Application.Dtos;

public sealed class EnvioWhatsapp
{
    public bool Enviado { get; init; }
    public bool Simulado { get; init; }
    public string Tipo { get; init; } = string.Empty;
    public string Texto { get; init; } = string.Empty;
    public string Error { get; init; } = string.Empty;
}
