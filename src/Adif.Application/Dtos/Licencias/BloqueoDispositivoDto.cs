// Bloqueo o desbloqueo de un dispositivo propio.
namespace Adif.Application.Dtos;

public sealed class BloqueoDispositivoDto
{
    public string Telefono { get; set; } = string.Empty;
    public bool Bloqueado { get; set; }
}
