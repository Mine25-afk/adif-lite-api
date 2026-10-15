using Adif.Domain.Common;
using Adif.Domain.Entities;

namespace Adif.UnitTests.Domain;

public class LicenciaTests
{
    private static readonly DateTime Hoy = new(2026, 3, 15);

    [Fact]
    public void Licencia_vencida_se_extiende_desde_hoy()
    {
        var dispositivo = new LicenciaDispositivo { Mensualidad = Hoy.AddMonths(-2) };

        dispositivo.ExtenderMensualidad(1, Hoy);

        dispositivo.Mensualidad.Should().Be(Hoy.AddMonths(1));
        dispositivo.Vigente(Hoy).Should().BeTrue();
    }

    [Fact]
    public void Licencia_vigente_acumula_sobre_el_vencimiento_actual()
    {
        var dispositivo = new LicenciaDispositivo { Mensualidad = Hoy.AddDays(10) };

        dispositivo.ExtenderMensualidad(2, Hoy);

        dispositivo.Mensualidad.Should().Be(Hoy.AddDays(10).AddMonths(2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(13)]
    public void Meses_fuera_de_rango_lanzan(int meses) => new LicenciaDispositivo().Invoking(d => d.ExtenderMensualidad(meses, Hoy)).Should().Throw<DomainException>();

    [Fact]
    public void Dispositivo_bloqueado_no_esta_vigente() => new LicenciaDispositivo { Mensualidad = Hoy.AddMonths(1), Bloqueado = true }.Vigente(Hoy).Should().BeFalse();

    [Fact]
    public void Codigo_de_activacion_solo_se_canjea_una_vez()
    {
        var codigo = new CodigoActivacion { Codigo = "ABC-123" };

        codigo.Canjear();

        codigo.Usado.Should().BeTrue();
        codigo.Invoking(c => c.Canjear()).Should().Throw<DomainException>();
    }
}
