using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.Domain.Entities;
using Adif.UnitTests.Support;
using Microsoft.Extensions.Logging.Abstractions;

namespace Adif.UnitTests.Application;

public class VentaServiceTests
{
    private readonly IVentaRepository _ventas = Substitute.For<IVentaRepository>();
    private readonly IProductoRepository _productos = Substitute.For<IProductoRepository>();
    private readonly ISocioRepository _socios = Substitute.For<ISocioRepository>();
    private readonly INegocioRepository _negocios = Substitute.For<INegocioRepository>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly VentaService _servicio;

    public VentaServiceTests()
    {
        _negocios.EsPropietarioAsync(Datos.NegocioId, Datos.Uid, Arg.Any<CancellationToken>()).Returns(true);
        _servicio = new VentaService(_ventas, _productos, _socios, new AccesoNegocio(_negocios), _uow, new RelojFijo(), NullLogger<VentaService>.Instance);
    }

    private void Catalogo(params Producto[] productos) => _productos.ObtenerVariosAsync(Datos.NegocioId, Arg.Any<IEnumerable<int>>(), Arg.Any<CancellationToken>()).Returns(productos.ToDictionary(p => p.Id));

    private static RegistrarVentaDto Venta(params LineaVentaDto[] lineas) => new() { NegocioId = Datos.NegocioId, Lineas = [.. lineas] };

    [Fact]
    public async Task Registrar_valida_guarda_una_vez_y_devuelve_totales()
    {
        var producto = Datos.Producto(id: 10, precio: 15m, stock: 4m);
        Catalogo(producto);

        var venta = await _servicio.RegistrarAsync(Datos.Uid, Venta(new LineaVentaDto { ProductoId = 10, Cantidad = 3 }));

        venta.Total.Should().Be(45m);
        venta.Fecha.Should().Be(RelojFijo.Hoy);
        producto.Stock.Should().Be(1m);
        await _ventas.Received(1).AgregarAsync(Arg.Is<Venta>(v => v.Total == 45m && v.Detalles.Count == 1), Arg.Any<CancellationToken>());
        await _uow.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registrar_en_negocio_ajeno_lanza_Forbidden_y_no_guarda()
    {
        var accion = () => _servicio.RegistrarAsync("uid-intruso", Venta(new LineaVentaDto { ProductoId = 10, Cantidad = 1 }));

        await accion.Should().ThrowAsync<ForbiddenException>();
        await _uow.DidNotReceive().GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Registrar_con_producto_inexistente_lanza_NotFound()
    {
        Catalogo(Datos.Producto(id: 10));

        var accion = () => _servicio.RegistrarAsync(Datos.Uid, Venta(new LineaVentaDto { ProductoId = 10, Cantidad = 1 }, new LineaVentaDto { ProductoId = 99, Cantidad = 1 }));

        await accion.Should().ThrowAsync<NotFoundException>().WithMessage("*99*");
    }

    [Fact]
    public async Task Registrar_a_credito_con_cliente_inexistente_lanza_NotFound()
    {
        Catalogo(Datos.Producto(id: 10));
        var dto = Venta(new LineaVentaDto { ProductoId = 10, Cantidad = 1 });
        dto.EsCredito = true;
        dto.SocioId = 77;

        await FluentActions.Invoking(() => _servicio.RegistrarAsync(Datos.Uid, dto)).Should().ThrowAsync<NotFoundException>().WithMessage("*cliente*");
    }

    [Fact]
    public async Task Anular_repone_stock_y_guarda()
    {
        var producto = Datos.Producto(id: 10, stock: 2m);
        var venta = new Venta { Id = 3, NegocioId = Datos.NegocioId, Total = 20m, Detalles = [new VentaDetalle { ProductoId = 10, Cantidad = 3m, Precio = 20m }] };
        _ventas.ObtenerConDetallesAsync(Datos.NegocioId, 3, Arg.Any<CancellationToken>()).Returns(venta);
        Catalogo(producto);

        var resultado = await _servicio.AnularAsync(Datos.Uid, Datos.NegocioId, 3);

        resultado.Estado.Should().Be(EstadoVenta.Anulada);
        producto.Stock.Should().Be(5m);
        await _uow.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Obtener_venta_inexistente_lanza_NotFound() => await FluentActions.Invoking(() => _servicio.ObtenerAsync(Datos.Uid, Datos.NegocioId, 404)).Should().ThrowAsync<NotFoundException>();
}
