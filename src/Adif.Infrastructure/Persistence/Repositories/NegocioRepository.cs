using Adif.Application.Abstractions.Persistence;
using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public sealed class NegocioRepository(AdifDbContext db) : Repository<Negocio>(db), INegocioRepository
{
    public Task<bool> EsPropietarioAsync(int negocioId, string uid, CancellationToken ct = default) => Set.AnyAsync(n => n.Id == negocioId && n.Propietario == uid && n.Activo, ct);

    public async Task<IReadOnlyList<Negocio>> ListarPorPropietarioAsync(string uid, CancellationToken ct = default) => await Set.AsNoTracking().Include(n => n.Numeros).Where(n => n.Propietario == uid).OrderBy(n => n.Nombre).ToListAsync(ct);
}
