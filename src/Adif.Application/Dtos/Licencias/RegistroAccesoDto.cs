// Acceso de un dispositivo a la app (queda en bitácora).
namespace Adif.Application.Dtos;

public sealed class RegistroAccesoDto
{
    public string Usuario { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
}
