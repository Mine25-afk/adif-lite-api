using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.Domain.Entities;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Application;

public class LicenciaServiceTests
{
    private readonly ILicenciaDispositivoRepository _dispositivos = Substitute.For<ILicenciaDispositivoRepository>();
    private readonly IRepository<LicenciaLog> _bitacora = Substitute.For<IRepository<LicenciaLog>>();
    private readonly IRepository<CodigoActivacion> _codigos = Substitute.For<IRepository<CodigoActivacion>>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly LicenciaService _servicio;

    public LicenciaServiceTests() => _servicio = new LicenciaService(_dispositivos, _bitacora, _codigos, _uow, new RelojFijo());

    [Fact]
    public async Task ComprarMensualidad_calcula_el_vencimiento_en_el_servidor()
    {
        var dispositivo = new LicenciaDispositivo { Usuario = "tienda", Telefono = "A1", Uid = Datos.Uid };
        _dispositivos.ListarPorUidAsync(Datos.Uid, Arg.Any<CancellationToken>()).Returns([dispositivo]);

        var resultado = await _servicio.ComprarMensualidadAsync(Datos.Uid, new CompraMensualidadDto { Telefono = "A1", Meses = 3 });

        resultado.Mensualidad.Should().Be(RelojFijo.Hoy.Date.AddMonths(3));
        resultado.Vigente.Should().BeTrue();
        await _bitacora.Received(1).AgregarAsync(Arg.Is<LicenciaLog>(l => l.Accion == "Mensualidad de 3 meses"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ComprarMensualidad_de_dispositivo_ajeno_lanza_NotFound()
    {
        _dispositivos.ListarPorUidAsync(Datos.Uid, Arg.Any<CancellationToken>()).Returns([]);

        await FluentActions.Invoking(() => _servicio.ComprarMensualidadAsync(Datos.Uid, new CompraMensualidadDto { Telefono = "A1", Meses = 1 })).Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task RegistrarAcceso_de_dispositivo_de_otra_cuenta_lanza_Forbidden()
    {
        _dispositivos.ObtenerAsync("tienda", "A1", Arg.Any<CancellationToken>()).Returns(new LicenciaDispositivo { Usuario = "tienda", Telefono = "A1", Uid = "otra-cuenta" });

        await FluentActions.Invoking(() => _servicio.RegistrarAccesoAsync(Datos.Uid, new RegistroAccesoDto { Usuario = "tienda", Telefono = "A1", Accion = "login" })).Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task RegistrarAcceso_nuevo_crea_dispositivo_y_bitacora()
    {
        var resultado = await _servicio.RegistrarAccesoAsync(Datos.Uid, new RegistroAccesoDto { Usuario = "tienda", Telefono = "A1", Accion = "login" });

        resultado.Bloqueado.Should().BeFalse();
        await _dispositivos.Received(1).AgregarAsync(Arg.Is<LicenciaDispositivo>(d => d.Uid == Datos.Uid), Arg.Any<CancellationToken>());
        await _bitacora.Received(1).AgregarAsync(Arg.Any<LicenciaLog>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CanjearCodigo_inexistente_lanza_NotFound() => await FluentActions.Invoking(() => _servicio.CanjearCodigoAsync(new ValidarCodigoDto { Codigo = "NO-EXISTE" })).Should().ThrowAsync<NotFoundException>();
}
