using Adif.Application.Dtos;
using Adif.Domain.Entities;

namespace Adif.Application.Abstractions.Persistence;

public interface IVentaRepository : IRepository<Venta>
{
    Task<Venta?> ObtenerConDetallesAsync(int negocioId, int ventaId, CancellationToken ct = default);
    Task<IReadOnlyList<Venta>> ListarCreditosAbiertosAsync(int negocioId, int socioId, CancellationToken ct = default);
    Task<decimal> CalcularSaldoClienteAsync(int negocioId, int socioId, CancellationToken ct = default);
    Task<IReadOnlyList<DeudorDto>> ListarDeudoresAsync(int negocioId, CancellationToken ct = default);
}
