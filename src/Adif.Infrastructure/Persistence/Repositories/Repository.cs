using System.Linq.Expressions;
using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public class Repository<T>(AdifDbContext db) : IRepository<T> where T : class
{
    protected AdifDbContext Db { get; } = db;
    protected DbSet<T> Set => Db.Set<T>();

    public async Task<T?> ObtenerPorIdAsync(object id, CancellationToken ct = default) => await Set.FindAsync([id], ct);

    public async Task<IReadOnlyList<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null, CancellationToken ct = default) => await Filtrar(filtro).ToListAsync(ct);

    public async Task<PagedResult<T>> PaginarAsync<TOrden>(Expression<Func<T, bool>>? filtro, Expression<Func<T, TOrden>> orden, bool descendente, int pagina, int tamano, CancellationToken ct = default)
    {
        var (p, t) = Paginacion.Normalizar(pagina, tamano);
        var query = Filtrar(filtro).AsNoTracking();
        var total = await query.CountAsync(ct);
        var ordenada = descendente ? query.OrderByDescending(orden) : query.OrderBy(orden);
        var items = await ordenada.Skip((p - 1) * t).Take(t).ToListAsync(ct);
        return new PagedResult<T> { Items = items, Pagina = p, TamanoPagina = t, Total = total };
    }

    public Task<bool> ExisteAsync(Expression<Func<T, bool>> filtro, CancellationToken ct = default) => Set.AnyAsync(filtro, ct);

    public Task<int> ContarAsync(Expression<Func<T, bool>>? filtro = null, CancellationToken ct = default) => Filtrar(filtro).CountAsync(ct);

    public async Task AgregarAsync(T entidad, CancellationToken ct = default) => await Set.AddAsync(entidad, ct);

    public void Eliminar(T entidad) => Set.Remove(entidad);

    private IQueryable<T> Filtrar(Expression<Func<T, bool>>? filtro) => filtro is null ? Set : Set.Where(filtro);
}
