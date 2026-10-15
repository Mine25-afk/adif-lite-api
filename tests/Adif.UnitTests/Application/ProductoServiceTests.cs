using Adif.Application.Abstractions.Persistence;
using Adif.Application.Abstractions.Services;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.Domain.Entities;
using Adif.UnitTests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace Adif.UnitTests.Application;

public class ProductoServiceTests
{
    private readonly IProductoRepository _productos = Substitute.For<IProductoRepository>();
    private readonly INegocioRepository _negocios = Substitute.For<INegocioRepository>();
    private readonly IImageStorage _imagenes = Substitute.For<IImageStorage>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ProductoService _servicio;

    public ProductoServiceTests()
    {
        _negocios.EsPropietarioAsync(Datos.NegocioId, Datos.Uid, Arg.Any<CancellationToken>()).Returns(true);
        _imagenes.GenerarUrlLectura(Arg.Any<string>(), Arg.Any<TimeSpan>()).Returns(c => $"https://blob.test/{c.Arg<string>()}?sas");
        _servicio = new ProductoService(_productos, new AccesoNegocio(_negocios), _imagenes, _uow, new RelojFijo(), NullLogger<ProductoService>.Instance);
    }

    [Fact]
    public async Task Crear_con_codigo_de_barras_repetido_lanza_Conflict()
    {
        _productos.ExisteCodigoBarraAsync(Datos.NegocioId, "7401", 0, Arg.Any<CancellationToken>()).Returns(true);

        var accion = () => _servicio.CrearAsync(Datos.Uid, new CrearProductoDto { NegocioId = Datos.NegocioId, Nombre = "Café", CodigoBarra = "7401" });

        await accion.Should().ThrowAsync<ConflictException>();
        await _uow.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Crear_valido_guarda_el_producto_del_negocio()
    {
        var dto = await _servicio.CrearAsync(Datos.Uid, new CrearProductoDto { NegocioId = Datos.NegocioId, Nombre = " Café molido ", Precio = 85m, Stock = 12m });

        dto.Nombre.Should().Be("Café molido");
        await _productos.Received(1).AgregarAsync(Arg.Is<Producto>(p => p.NegocioId == Datos.NegocioId && p.Stock == 12m), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AjustarStock_aplica_la_diferencia()
    {
        var producto = Datos.Producto(id: 4, stock: 10m);
        _productos.ObtenerDelNegocioAsync(Datos.NegocioId, 4, Arg.Any<CancellationToken>()).Returns(producto);

        var dto = await _servicio.AjustarStockAsync(Datos.Uid, new AjusteStockDto { NegocioId = Datos.NegocioId, ProductoId = 4, Diferencia = -3m, Motivo = "merma" });

        dto.Stock.Should().Be(7m);
    }

    [Fact]
    public async Task SubirImagen_guarda_ruta_por_negocio_y_borra_la_anterior()
    {
        var producto = Datos.Producto(id: 4);
        producto.ImagenBlob = "1/4.png";
        _productos.ObtenerDelNegocioAsync(Datos.NegocioId, 4, Arg.Any<CancellationToken>()).Returns(producto);

        var imagen = await _servicio.SubirImagenAsync(Datos.Uid, Datos.NegocioId, 4, Stream.Null, "image/jpeg", ".JPG");

        producto.ImagenBlob.Should().Be("1/4.jpg");
        imagen.Url.Should().StartWith("https://blob.test/1/4.jpg");
        await _imagenes.Received(1).EliminarAsync("1/4.png", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ObtenerImagen_sin_imagen_lanza_NotFound()
    {
        _productos.ObtenerDelNegocioAsync(Datos.NegocioId, 4, Arg.Any<CancellationToken>()).Returns(Datos.Producto(id: 4));

        await FluentActions.Invoking(() => _servicio.ObtenerImagenAsync(Datos.Uid, Datos.NegocioId, 4)).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Listar_en_negocio_ajeno_lanza_Forbidden() => await FluentActions.Invoking(() => _servicio.ListarAsync("otro", Datos.NegocioId, null, true, 1, 20)).Should().ThrowAsync<ForbiddenException>();
}
