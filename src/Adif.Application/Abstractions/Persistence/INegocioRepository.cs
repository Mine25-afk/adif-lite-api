using Adif.Domain.Entities;

namespace Adif.Application.Abstractions.Persistence;

public interface INegocioRepository : IRepository<Negocio>
{
    Task<bool> EsPropietarioAsync(int negocioId, string uid, CancellationToken ct = default);
    Task<IReadOnlyList<Negocio>> ListarPorPropietarioAsync(string uid, CancellationToken ct = default);
}
