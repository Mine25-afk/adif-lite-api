// Empresa con sus números de WhatsApp (sin credenciales).
namespace Adif.Application.Dtos;

public sealed class NegocioDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public string Direccion { get; init; } = string.Empty;
    public string Rtn { get; init; } = string.Empty;
    public bool Activo { get; init; }
    public IReadOnlyList<NumeroWhatsappDto> Numeros { get; init; } = [];
}
