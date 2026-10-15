// Autorización de un teléfono para operar el fiado de un negocio.
namespace Adif.Application.Dtos;

public sealed class AgregarOperadorDto
{
    public int NegocioId { get; set; }
    public string Telefono { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}
