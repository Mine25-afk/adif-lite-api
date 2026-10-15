using Adif.Application.Dtos;

namespace Adif.Application.Services;

// Licencia mensual de la app por dispositivo; siempre acotada al uid del token.
public interface ILicenciaService
{
    Task<DispositivoDto> RegistrarAccesoAsync(string uid, RegistroAccesoDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<DispositivoDto>> ListarDispositivosAsync(string uid, CancellationToken ct = default);
    Task<DispositivoDto> CrearCuentaAsync(string uid, CrearCuentaDto dto, CancellationToken ct = default);
    Task<DispositivoDto> ComprarMensualidadAsync(string uid, CompraMensualidadDto dto, CancellationToken ct = default);
    Task<DispositivoDto> CambiarBloqueoAsync(string uid, BloqueoDispositivoDto dto, CancellationToken ct = default);
    Task CanjearCodigoAsync(ValidarCodigoDto dto, CancellationToken ct = default);
}
