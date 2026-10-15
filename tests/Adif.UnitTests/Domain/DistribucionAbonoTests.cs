using Adif.Domain.Common;
using Adif.Domain.Fiado;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Domain;

public class DistribucionAbonoTests
{
    private static readonly DateTime Hoy = new(2026, 3, 15);

    [Fact]
    public void Aplica_primero_al_credito_mas_antiguo()
    {
        var nuevo = Datos.Credito(2, 50m, Hoy.AddDays(-1));
        var viejo = Datos.Credito(1, 30m, Hoy.AddDays(-10));

        var resultado = DistribucionAbono.Aplicar([nuevo, viejo], 40m, Hoy, "abono");

        viejo.Saldo.Should().Be(0m);
        nuevo.Saldo.Should().Be(40m);
        resultado.Aplicado.Should().Be(40m);
        resultado.Sobrante.Should().Be(0m);
        resultado.Abonos.Select(a => (a.VentaId, a.Monto)).Should().Equal((1, 30m), (2, 10m));
    }

    [Fact]
    public void Si_el_pago_supera_la_deuda_devuelve_el_sobrante()
    {
        var credito = Datos.Credito(1, 25m, Hoy);

        var resultado = DistribucionAbono.Aplicar([credito], 100m, Hoy, "abono");

        resultado.Aplicado.Should().Be(25m);
        resultado.Sobrante.Should().Be(75m);
        resultado.Abonos.Should().ContainSingle();
    }

    [Fact]
    public void Sin_creditos_abiertos_no_genera_abonos()
    {
        var resultado = DistribucionAbono.Aplicar([Datos.Credito(1, 0m, Hoy)], 10m, Hoy, "abono");

        resultado.Aplicado.Should().Be(0m);
        resultado.Abonos.Should().BeEmpty();
    }

    [Fact]
    public void Monto_no_positivo_lanza() => FluentActions.Invoking(() => DistribucionAbono.Aplicar([], 0m, Hoy, "x")).Should().Throw<DomainException>();
}
