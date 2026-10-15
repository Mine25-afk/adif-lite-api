using Adif.Application.Abstractions.Persistence;
using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public sealed class LicenciaDispositivoRepository(AdifDbContext db) : Repository<LicenciaDispositivo>(db), ILicenciaDispositivoRepository
{
    public async Task<LicenciaDispositivo?> ObtenerAsync(string usuario, string telefono, CancellationToken ct = default) => await Set.FindAsync([usuario, telefono], ct);

    public async Task<IReadOnlyList<LicenciaDispositivo>> ListarPorUidAsync(string uid, CancellationToken ct = default) => await Set.Where(d => d.Uid == uid).OrderBy(d => d.Telefono).ToListAsync(ct);
}
