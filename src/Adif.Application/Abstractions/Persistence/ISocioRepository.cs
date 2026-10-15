using Adif.Application.Dtos;
using Adif.Domain.Entities;

namespace Adif.Application.Abstractions.Persistence;

public interface ISocioRepository : IRepository<Socio>
{
    Task<Socio?> ObtenerClienteAsync(int negocioId, int socioId, CancellationToken ct = default);
    Task<Socio?> ObtenerClientePorNombreAsync(int negocioId, string nombre, CancellationToken ct = default);
    Task<IReadOnlyList<ClienteFiadoDto>> BuscarClientesConSaldoAsync(int negocioId, string texto, int maximo, CancellationToken ct = default);
}
