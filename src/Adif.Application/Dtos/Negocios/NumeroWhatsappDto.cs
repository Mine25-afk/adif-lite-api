// Número de WhatsApp de una empresa.
namespace Adif.Application.Dtos;

public sealed class NumeroWhatsappDto
{
    public int Id { get; init; }
    public string PhoneNumberId { get; init; } = string.Empty;
    public string DisplayNumber { get; init; } = string.Empty;
    public string WabaId { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public bool FirmaConfigurada { get; init; }
}
