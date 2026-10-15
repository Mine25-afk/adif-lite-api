using Adif.Application.Common;
using Adif.Application.Dtos;

namespace Adif.Application.Services;

public interface IProductoService
{
    Task<PagedResult<ProductoDto>> ListarAsync(string uid, int negocioId, string? buscar, bool soloActivos, int pagina, int tamano, CancellationToken ct = default);
    Task<ProductoDto> ObtenerAsync(string uid, int negocioId, int productoId, CancellationToken ct = default);
    Task<ProductoDto> CrearAsync(string uid, CrearProductoDto dto, CancellationToken ct = default);
    Task<ProductoDto> ActualizarAsync(string uid, int productoId, ActualizarProductoDto dto, CancellationToken ct = default);
    Task<ProductoDto> AjustarStockAsync(string uid, AjusteStockDto dto, CancellationToken ct = default);
    Task<IReadOnlyList<ProductoDto>> ListarStockBajoAsync(string uid, int negocioId, CancellationToken ct = default);
    Task<ImagenProductoDto> SubirImagenAsync(string uid, int negocioId, int productoId, Stream contenido, string contentType, string extension, CancellationToken ct = default);
    Task<ImagenProductoDto> ObtenerImagenAsync(string uid, int negocioId, int productoId, CancellationToken ct = default);
}
