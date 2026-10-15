using System.Linq.Expressions;
using Adif.Application.Common;

namespace Adif.Application.Abstractions.Persistence;

// Repositorio genérico: CRUD y paginación sin exponer EF Core a la capa de aplicación.
public interface IRepository<T> where T : class
{
    Task<T?> ObtenerPorIdAsync(object id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListarAsync(Expression<Func<T, bool>>? filtro = null, CancellationToken ct = default);
    Task<PagedResult<T>> PaginarAsync<TOrden>(Expression<Func<T, bool>>? filtro, Expression<Func<T, TOrden>> orden, bool descendente, int pagina, int tamano, CancellationToken ct = default);
    Task<bool> ExisteAsync(Expression<Func<T, bool>> filtro, CancellationToken ct = default);
    Task<int> ContarAsync(Expression<Func<T, bool>>? filtro = null, CancellationToken ct = default);
    Task AgregarAsync(T entidad, CancellationToken ct = default);
    void Eliminar(T entidad);
}
