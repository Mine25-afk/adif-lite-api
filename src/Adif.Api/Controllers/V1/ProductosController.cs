using Adif.Application.Common;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers.V1;

[Route(ApiRoutes.V1 + "/productos")]
public sealed class ProductosController(IProductoService productos) : ApiControllerBase
{
    private const long TamanoMaximoImagen = 5 * 1024 * 1024;

    // El tipo real se toma del Content-Type permitido, no de la extensión que mande el cliente.
    private static readonly Dictionary<string, string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ["image/jpeg"] = ".jpg",
        ["image/png"] = ".png",
        ["image/webp"] = ".webp"
    };

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductoDto>>> Listar(int negocioId, string? buscar, bool soloActivos = true, int pagina = 1, int tamano = Paginacion.TamanoPorDefecto, CancellationToken ct = default) => Ok(await productos.ListarAsync(Uid, negocioId, buscar, soloActivos, pagina, tamano, ct));

    [HttpGet("detalle")]
    public async Task<ActionResult<ProductoDto>> Obtener(int negocioId, int id, CancellationToken ct) => Ok(await productos.ObtenerAsync(Uid, negocioId, id, ct));

    [HttpPost]
    [ProducesResponseType<ProductoDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear(CrearProductoDto dto, CancellationToken ct) => StatusCode(StatusCodes.Status201Created, await productos.CrearAsync(Uid, dto, ct));

    [HttpPut]
    public async Task<ActionResult<ProductoDto>> Actualizar(int id, ActualizarProductoDto dto, CancellationToken ct) => Ok(await productos.ActualizarAsync(Uid, id, dto, ct));

    [HttpPost("ajuste-stock")]
    public async Task<ActionResult<ProductoDto>> AjustarStock(AjusteStockDto dto, CancellationToken ct) => Ok(await productos.AjustarStockAsync(Uid, dto, ct));

    [HttpGet("stock-bajo")]
    public async Task<ActionResult<IReadOnlyList<ProductoDto>>> StockBajo(int negocioId, CancellationToken ct) => Ok(await productos.ListarStockBajoAsync(Uid, negocioId, ct));

    [HttpPost("imagen")]
    [RequestSizeLimit(TamanoMaximoImagen + 64 * 1024)]
    public async Task<ActionResult<ImagenProductoDto>> SubirImagen(int negocioId, int productoId, IFormFile archivo, CancellationToken ct)
    {
        if (archivo.Length == 0 || archivo.Length > TamanoMaximoImagen) return ValidationProblem(Error(nameof(archivo), "La imagen debe pesar entre 1 byte y 5 MB."));
        if (!ExtensionesPermitidas.TryGetValue(archivo.ContentType, out var extension)) return ValidationProblem(Error(nameof(archivo), "Solo se aceptan imágenes JPEG, PNG o WebP."));

        await using var contenido = archivo.OpenReadStream();
        return Ok(await productos.SubirImagenAsync(Uid, negocioId, productoId, contenido, archivo.ContentType, extension, ct));
    }

    [HttpGet("imagen")]
    public async Task<ActionResult<ImagenProductoDto>> ObtenerImagen(int negocioId, int productoId, CancellationToken ct) => Ok(await productos.ObtenerImagenAsync(Uid, negocioId, productoId, ct));

    private static ValidationProblemDetails Error(string campo, string mensaje) => new(new Dictionary<string, string[]> { [campo] = [mensaje] });
}
