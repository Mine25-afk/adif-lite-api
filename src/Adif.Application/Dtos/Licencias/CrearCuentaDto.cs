// Alta de la cuenta de licencia para el dispositivo actual.
namespace Adif.Application.Dtos;

public sealed class CrearCuentaDto
{
    public string Usuario { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
}
