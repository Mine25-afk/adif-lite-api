using Adif.Application.Abstractions.Persistence;
using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public sealed class ProductoRepository(AdifDbContext db) : Repository<Producto>(db), IProductoRepository
{
    public Task<Producto?> ObtenerDelNegocioAsync(int negocioId, int productoId, CancellationToken ct = default) => Set.FirstOrDefaultAsync(p => p.NegocioId == negocioId && p.Id == productoId, ct);

    public async Task<IReadOnlyDictionary<int, Producto>> ObtenerVariosAsync(int negocioId, IEnumerable<int> ids, CancellationToken ct = default)
    {
        var lista = ids.ToList();
        return await Set.Where(p => p.NegocioId == negocioId && lista.Contains(p.Id)).ToDictionaryAsync(p => p.Id, ct);
    }

    public async Task<IReadOnlyList<Producto>> ListarStockBajoAsync(int negocioId, CancellationToken ct = default) =>
        await Set.AsNoTracking().Where(p => p.NegocioId == negocioId && p.Activo && p.StockMinimo > 0 && p.Stock <= p.StockMinimo).OrderBy(p => p.Stock).ThenBy(p => p.Nombre).ToListAsync(ct);

    public Task<bool> ExisteCodigoBarraAsync(int negocioId, string codigoBarra, int excluirId, CancellationToken ct = default) => Set.AnyAsync(p => p.NegocioId == negocioId && p.CodigoBarra == codigoBarra && p.Id != excluirId, ct);
}
