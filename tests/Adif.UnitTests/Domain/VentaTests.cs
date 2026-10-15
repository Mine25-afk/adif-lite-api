using Adif.Domain.Common;
using Adif.Domain.Entities;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Domain;

public class VentaTests
{
    private static readonly DateTime Fecha = new(2026, 3, 15, 9, 30, 0);

    private static Venta Registrar(IReadOnlyCollection<LineaVenta> lineas, bool credito = false, bool isv = false, int socioId = 0, int dias = 0) =>
        Venta.Registrar(Datos.NegocioId, socioId, Fecha, lineas, credito, isv, MetodoPago.Efectivo, "  nota  ", dias);

    [Fact]
    public void Registrar_de_contado_calcula_total_y_descuenta_stock()
    {
        var arroz = Datos.Producto(id: 1, precio: 12.50m, stock: 10m);
        var aceite = Datos.Producto(id: 2, precio: 45m, stock: 3m);

        var venta = Registrar([new(arroz, 2m), new(aceite, 1m)]);

        venta.Total.Should().Be(70m);
        venta.Isv.Should().Be(0m);
        venta.Saldo.Should().Be(0m);
        venta.Comentario.Should().Be("nota");
        venta.Detalles.Should().HaveCount(2);
        arroz.Stock.Should().Be(8m);
        aceite.Stock.Should().Be(2m);
    }

    [Fact]
    public void Registrar_con_isv_agrega_15_por_ciento_redondeado()
    {
        var venta = Registrar([new(Datos.Producto(precio: 33.33m), 1m)], isv: true);

        venta.Subtotal.Should().Be(33.33m);
        venta.Isv.Should().Be(5.00m);
        venta.Total.Should().Be(38.33m);
    }

    [Fact]
    public void Registrar_usa_precio_de_mayoreo_cuando_se_pide()
    {
        var venta = Registrar([new(Datos.Producto(precio: 20m, mayoreo: 17m), 10m, PrecioMayoreo: true)]);

        venta.Total.Should().Be(170m);
    }

    [Fact]
    public void Registrar_a_credito_deja_saldo_y_vencimiento()
    {
        var venta = Registrar([new(Datos.Producto(precio: 100m), 1m)], credito: true, socioId: 5, dias: 15);

        venta.EsCredito.Should().BeTrue();
        venta.Saldo.Should().Be(100m);
        venta.FechaVence.Should().Be(Fecha.Date.AddDays(15));
    }

    [Fact]
    public void Registrar_a_credito_sin_cliente_lanza() => FluentActions.Invoking(() => Registrar([new(Datos.Producto(), 1m)], credito: true, socioId: 0)).Should().Throw<DomainException>().WithMessage("*cliente*");

    [Fact]
    public void Registrar_sin_lineas_lanza() => FluentActions.Invoking(() => Registrar([])).Should().Throw<DomainException>();

    [Fact]
    public void Registrar_con_producto_de_otro_negocio_lanza() => FluentActions.Invoking(() => Registrar([new(Datos.Producto(negocioId: 99), 1m)])).Should().Throw<DomainException>().WithMessage("*no pertenece*");

    [Fact]
    public void Anular_repone_el_stock_y_marca_la_venta()
    {
        var producto = Datos.Producto(id: 7, stock: 5m);
        var venta = Registrar([new(producto, 2m)]);

        venta.Anular(new Dictionary<int, Producto> { [7] = producto });

        venta.Estado.Should().Be(EstadoVenta.Anulada);
        producto.Stock.Should().Be(5m);
    }

    [Fact]
    public void Anular_un_credito_con_abonos_lanza()
    {
        var venta = Datos.Credito(1, 100m, Fecha);
        venta.AplicarAbono(10m);

        venta.Invoking(v => v.Anular(new Dictionary<int, Producto>())).Should().Throw<DomainException>().WithMessage("*abonos*");
    }

    [Fact]
    public void Anular_dos_veces_lanza()
    {
        var venta = Registrar([new(Datos.Producto(), 1m)]);
        venta.Anular(new Dictionary<int, Producto>());

        venta.Invoking(v => v.Anular(new Dictionary<int, Producto>())).Should().Throw<DomainException>();
    }

    [Fact]
    public void AplicarAbono_nunca_aplica_mas_que_el_saldo()
    {
        var venta = Datos.Credito(1, 30m, Fecha);

        venta.AplicarAbono(50m).Should().Be(30m);
        venta.Saldo.Should().Be(0m);
    }

    [Fact]
    public void CargoFiado_crea_credito_por_el_monto()
    {
        var cargo = Venta.CargoFiado(Datos.NegocioId, 5, 75.456m, null, Fecha);

        cargo.EsCredito.Should().BeTrue();
        cargo.Total.Should().Be(75.46m);
        cargo.Saldo.Should().Be(75.46m);
        cargo.Comentario.Should().Be("Crédito por WhatsApp");
    }
}
