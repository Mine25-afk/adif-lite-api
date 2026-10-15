using Adif.Application.Abstractions.Persistence;

namespace Adif.Infrastructure.Persistence;

public sealed class UnitOfWork(AdifDbContext db) : IUnitOfWork
{
    public Task<int> GuardarCambiosAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
