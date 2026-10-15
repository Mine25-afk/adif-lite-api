using Adif.Application.Dtos;

namespace Adif.Application.Services;

public interface INegocioService
{
    Task<NegocioDto> CrearAsync(string uid, CrearNegocioDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<NegocioDto>> ListarMiosAsync(string uid, CancellationToken ct = default);
    Task<bool> AgregarOperadorAsync(string uid, AgregarOperadorDto dto, CancellationToken ct = default);
}
