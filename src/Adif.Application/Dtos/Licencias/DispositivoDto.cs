// Dispositivo de un usuario con el estado de su licencia.
using Adif.Domain.Common;

namespace Adif.Application.Dtos;

public sealed class DispositivoDto
{
    public string Usuario { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public bool Bloqueado { get; init; }
    public DateTime Mensualidad { get; init; } = Fechas.SinFecha;
    public int Cantidad { get; init; }
    public bool Vigente { get; init; }
}
