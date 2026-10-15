using Adif.Application.Abstractions.Persistence;
using Adif.Application.Abstractions.Services;
using Adif.Application.Common;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Mapping;
using Adif.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Adif.Application.Services;

public sealed class ProductoService(
    IProductoRepository productos,
    IAccesoNegocio acceso,
    IImageStorage imagenes,
    IUnitOfWork uow,
    TimeProvider reloj,
    ILogger<ProductoService> logger) : IProductoService
{
    // Las URLs firmadas caducan pronto: el contenedor es privado.
    private static readonly TimeSpan VigenciaImagen = TimeSpan.FromHours(2);

    public async Task<PagedResult<ProductoDto>> ListarAsync(string uid, int negocioId, string? buscar, bool soloActivos, int pagina, int tamano, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);

        var texto = (buscar ?? string.Empty).Trim().ToLower();
        var (p, t) = Paginacion.Normalizar(pagina, tamano);
        var resultado = await productos.PaginarAsync(
            x => x.NegocioId == negocioId && (!soloActivos || x.Activo) && (texto == "" || x.Nombre.ToLower().Contains(texto) || x.CodigoBarra == texto),
            x => x.Nombre, false, p, t, ct);
        return resultado.Map(x => x.ADto());
    }

    public async Task<ProductoDto> ObtenerAsync(string uid, int negocioId, int productoId, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);
        return (await Cargar(negocioId, productoId, ct)).ADto();
    }

    public async Task<ProductoDto> CrearAsync(string uid, CrearProductoDto dto, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, dto.NegocioId, ct);
        await ValidarCodigoBarraLibre(dto.NegocioId, dto.CodigoBarra, 0, ct);

        var producto = new Producto
        {
            NegocioId = dto.NegocioId,
            Nombre = dto.Nombre.Trim(),
            Precio = dto.Precio,
            PrecioMayoreo = dto.PrecioMayoreo,
            Costo = dto.Costo,
            Stock = dto.Stock,
            StockMinimo = dto.StockMinimo,
            Unidad = dto.Unidad.Trim(),
            CodigoBarra = dto.CodigoBarra.Trim(),
            CategoriaId = dto.CategoriaId,
            Notas = dto.Notas.Trim()
        };
        await productos.AgregarAsync(producto, ct);
        await uow.GuardarCambiosAsync(ct);
        logger.LogInformation("Producto {ProductoId} creado en negocio {NegocioId}", producto.Id, producto.NegocioId);
        return producto.ADto();
    }

    public async Task<ProductoDto> ActualizarAsync(string uid, int productoId, ActualizarProductoDto dto, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, dto.NegocioId, ct);
        var producto = await Cargar(dto.NegocioId, productoId, ct);
        await ValidarCodigoBarraLibre(dto.NegocioId, dto.CodigoBarra, productoId, ct);

        producto.Nombre = dto.Nombre.Trim();
        producto.Precio = dto.Precio;
        producto.PrecioMayoreo = dto.PrecioMayoreo;
        producto.Costo = dto.Costo;
        producto.StockMinimo = dto.StockMinimo;
        producto.Unidad = dto.Unidad.Trim();
        producto.CodigoBarra = dto.CodigoBarra.Trim();
        producto.CategoriaId = dto.CategoriaId;
        producto.Notas = dto.Notas.Trim();
        producto.Activo = dto.Activo;

        await uow.GuardarCambiosAsync(ct);
        return producto.ADto();
    }

    public async Task<ProductoDto> AjustarStockAsync(string uid, AjusteStockDto dto, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, dto.NegocioId, ct);
        var producto = await Cargar(dto.NegocioId, dto.ProductoId, ct);

        producto.AjustarStock(dto.Diferencia);
        await uow.GuardarCambiosAsync(ct);
        logger.LogInformation("Ajuste de stock {Diferencia} al producto {ProductoId}: {Motivo}", dto.Diferencia, producto.Id, dto.Motivo);
        return producto.ADto();
    }

    public async Task<IReadOnlyList<ProductoDto>> ListarStockBajoAsync(string uid, int negocioId, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);
        return (await productos.ListarStockBajoAsync(negocioId, ct)).Select(p => p.ADto()).ToList();
    }

    public async Task<ImagenProductoDto> SubirImagenAsync(string uid, int negocioId, int productoId, Stream contenido, string contentType, string extension, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);
        var producto = await Cargar(negocioId, productoId, ct);

        // La ruta empieza por el negocio para aislar los archivos de cada tenant.
        var ruta = $"{producto.NegocioId}/{producto.Id}{extension.ToLowerInvariant()}";
        var anterior = producto.ImagenBlob;
        await imagenes.SubirAsync(ruta, contenido, contentType, ct);
        producto.ImagenBlob = ruta;
        await uow.GuardarCambiosAsync(ct);

        if (anterior.Length > 0 && anterior != ruta) await imagenes.EliminarAsync(anterior, ct);
        return Firmar(producto);
    }

    public async Task<ImagenProductoDto> ObtenerImagenAsync(string uid, int negocioId, int productoId, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);
        var producto = await Cargar(negocioId, productoId, ct);
        if (producto.ImagenBlob.Length == 0) throw new NotFoundException("El producto no tiene imagen.");
        return Firmar(producto);
    }

    private ImagenProductoDto Firmar(Producto producto)
    {
        var url = imagenes.GenerarUrlLectura(producto.ImagenBlob, VigenciaImagen) ?? throw new InvalidOperationException("El almacenamiento no puede firmar URLs de lectura.");
        return new ImagenProductoDto { ProductoId = producto.Id, Url = url, ExpiraEn = reloj.GetLocalNow().DateTime.Add(VigenciaImagen) };
    }

    private async Task<Producto> Cargar(int negocioId, int productoId, CancellationToken ct) => await productos.ObtenerDelNegocioAsync(negocioId, productoId, ct) ?? throw new NotFoundException($"No existe el producto {productoId}.");

    private async Task ValidarCodigoBarraLibre(int negocioId, string codigo, int excluirId, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(codigo) && await productos.ExisteCodigoBarraAsync(negocioId, codigo.Trim(), excluirId, ct))
            throw new ConflictException($"Ya existe un producto con el código de barras {codigo.Trim()}.");
    }
}
