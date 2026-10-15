using Adif.Application.Common;
using Adif.Application.Dtos;

namespace Adif.Application.Services;

public interface IVentaService
{
    Task<VentaDto> RegistrarAsync(string uid, RegistrarVentaDto dto, CancellationToken ct = default);
    Task<PagedResult<VentaDto>> ListarAsync(string uid, int negocioId, DateTime? desde, DateTime? hasta, int pagina, int tamano, CancellationToken ct = default);
    Task<VentaDto> ObtenerAsync(string uid, int negocioId, int ventaId, CancellationToken ct = default);
    Task<VentaDto> AnularAsync(string uid, int negocioId, int ventaId, CancellationToken ct = default);
}
