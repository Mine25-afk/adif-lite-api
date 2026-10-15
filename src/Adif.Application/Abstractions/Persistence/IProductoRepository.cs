using Adif.Domain.Entities;

namespace Adif.Application.Abstractions.Persistence;

public interface IProductoRepository : IRepository<Producto>
{
    Task<Producto?> ObtenerDelNegocioAsync(int negocioId, int productoId, CancellationToken ct = default);
    Task<IReadOnlyDictionary<int, Producto>> ObtenerVariosAsync(int negocioId, IEnumerable<int> ids, CancellationToken ct = default);
    Task<IReadOnlyList<Producto>> ListarStockBajoAsync(int negocioId, CancellationToken ct = default);
    Task<bool> ExisteCodigoBarraAsync(int negocioId, string codigoBarra, int excluirId, CancellationToken ct = default);
}
