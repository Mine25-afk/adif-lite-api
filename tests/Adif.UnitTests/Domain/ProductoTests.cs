using Adif.Domain.Common;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Domain;

public class ProductoTests
{
    [Fact]
    public void DescontarStock_reduce_el_inventario()
    {
        var producto = Datos.Producto(stock: 10m);

        producto.DescontarStock(2.5m);

        producto.Stock.Should().Be(7.5m);
    }

    [Fact]
    public void DescontarStock_con_stock_insuficiente_lanza_y_no_modifica()
    {
        var producto = Datos.Producto(stock: 1m);

        var accion = () => producto.DescontarStock(2m);

        accion.Should().Throw<DomainException>().WithMessage("*Stock insuficiente*");
        producto.Stock.Should().Be(1m);
    }

    [Fact]
    public void DescontarStock_de_producto_inactivo_lanza()
    {
        var producto = Datos.Producto();
        producto.Activo = false;

        producto.Invoking(p => p.DescontarStock(1m)).Should().Throw<DomainException>().WithMessage("*inactivo*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void DescontarStock_con_cantidad_no_positiva_lanza(decimal cantidad) => Datos.Producto().Invoking(p => p.DescontarStock(cantidad)).Should().Throw<DomainException>();

    [Fact]
    public void AjustarStock_no_puede_dejar_inventario_negativo()
    {
        var producto = Datos.Producto(stock: 3m);

        producto.Invoking(p => p.AjustarStock(-4m)).Should().Throw<DomainException>();
        producto.AjustarStock(-3m);

        producto.Stock.Should().Be(0m);
    }

    [Fact]
    public void StockBajo_solo_aplica_con_minimo_configurado()
    {
        var sinMinimo = Datos.Producto(stock: 0m);
        var bajo = Datos.Producto(stock: 2m);
        bajo.StockMinimo = 5m;

        sinMinimo.StockBajo.Should().BeFalse();
        bajo.StockBajo.Should().BeTrue();
    }

    [Fact]
    public void PrecioPara_usa_mayoreo_solo_si_existe()
    {
        Datos.Producto(precio: 20m, mayoreo: 18m).PrecioPara(mayoreo: true).Should().Be(18m);
        Datos.Producto(precio: 20m, mayoreo: 0m).PrecioPara(mayoreo: true).Should().Be(20m);
        Datos.Producto(precio: 20m, mayoreo: 18m).PrecioPara(mayoreo: false).Should().Be(20m);
    }
}
