using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Adif.Domain.Entities;
using Adif.UnitTests.Support;

namespace Adif.UnitTests.Application;

public class FiadoServiceTests
{
    private readonly ISocioRepository _socios = Substitute.For<ISocioRepository>();
    private readonly IVentaRepository _ventas = Substitute.For<IVentaRepository>();
    private readonly IRepository<Abono> _abonos = Substitute.For<IRepository<Abono>>();
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly FiadoService _servicio;

    public FiadoServiceTests()
    {
        _socios.ObtenerClienteAsync(Datos.NegocioId, 5, Arg.Any<CancellationToken>()).Returns(Datos.Cliente(5));
        _servicio = new FiadoService(_socios, _ventas, _abonos, _uow, new RelojFijo());
    }

    [Fact]
    public async Task CrearCliente_existente_no_lo_duplica()
    {
        _socios.ObtenerClientePorNombreAsync(Datos.NegocioId, "María López", Arg.Any<CancellationToken>()).Returns(Datos.Cliente(5));
        _ventas.CalcularSaldoClienteAsync(Datos.NegocioId, 5, Arg.Any<CancellationToken>()).Returns(40m);

        var cliente = await _servicio.CrearClienteAsync(Datos.NegocioId, "  María López ", null);

        cliente.YaExistia.Should().BeTrue();
        cliente.Saldo.Should().Be(40m);
        await _socios.DidNotReceive().AgregarAsync(Arg.Any<Socio>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CrearCliente_nuevo_se_guarda_sin_deuda()
    {
        var cliente = await _servicio.CrearClienteAsync(Datos.NegocioId, "Juan Pérez", "50499990000");

        cliente.YaExistia.Should().BeFalse();
        cliente.Saldo.Should().Be(0m);
        await _socios.Received(1).AgregarAsync(Arg.Is<Socio>(s => s.Nombre == "Juan Pérez" && s.NegocioId == Datos.NegocioId), Arg.Any<CancellationToken>());
        await _uow.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AgregarCargo_suma_al_saldo_previo()
    {
        _ventas.CalcularSaldoClienteAsync(Datos.NegocioId, 5, Arg.Any<CancellationToken>()).Returns(20m);

        var cliente = await _servicio.AgregarCargoAsync(Datos.NegocioId, 5, 30m, "Leche y pan");

        cliente.Saldo.Should().Be(50m);
        await _ventas.Received(1).AgregarAsync(Arg.Is<Venta>(v => v.EsCredito && v.Saldo == 30m && v.Comentario == "Leche y pan"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegistrarAbono_reparte_fifo_y_persiste_cada_abono()
    {
        var viejo = Datos.Credito(1, 30m, RelojFijo.Hoy.AddDays(-5));
        var nuevo = Datos.Credito(2, 50m, RelojFijo.Hoy.AddDays(-1));
        _ventas.ListarCreditosAbiertosAsync(Datos.NegocioId, 5, Arg.Any<CancellationToken>()).Returns([viejo, nuevo]);

        var resultado = await _servicio.RegistrarAbonoAsync(Datos.NegocioId, 5, 40m);

        resultado.Aplicado.Should().Be(40m);
        resultado.SaldoRestante.Should().Be(40m);
        resultado.Mensaje.Should().Be("Abono aplicado.");
        await _abonos.Received(2).AgregarAsync(Arg.Any<Abono>(), Arg.Any<CancellationToken>());
        await _uow.Received(1).GuardarCambiosAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegistrarAbono_mayor_a_la_deuda_avisa_el_sobrante()
    {
        _ventas.ListarCreditosAbiertosAsync(Datos.NegocioId, 5, Arg.Any<CancellationToken>()).Returns([Datos.Credito(1, 10m, RelojFijo.Hoy)]);

        var resultado = await _servicio.RegistrarAbonoAsync(Datos.NegocioId, 5, 25m);

        resultado.Aplicado.Should().Be(10m);
        resultado.Sobrante.Should().Be(15m);
        resultado.Mensaje.Should().Contain("superaba");
    }

    [Fact]
    public async Task ResolverCliente_ambiguo_lanza_Conflict()
    {
        _socios.BuscarClientesConSaldoAsync(Datos.NegocioId, "Ana", Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns([new ClienteFiadoDto { Id = 1 }, new ClienteFiadoDto { Id = 2 }]);

        await FluentActions.Invoking(() => _servicio.ResolverClienteAsync(Datos.NegocioId, 0, "Ana")).Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task ResolverCliente_por_nombre_unico_devuelve_su_id()
    {
        _socios.BuscarClientesConSaldoAsync(Datos.NegocioId, "Ana", Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns([new ClienteFiadoDto { Id = 8 }]);

        (await _servicio.ResolverClienteAsync(Datos.NegocioId, 0, "Ana")).Should().Be(8);
    }

    [Fact]
    public async Task Operar_sobre_cliente_inexistente_lanza_NotFound()
    {
        await FluentActions.Invoking(() => _servicio.RegistrarAbonoAsync(Datos.NegocioId, 999, 10m)).Should().ThrowAsync<NotFoundException>();
        await FluentActions.Invoking(() => _servicio.ResolverClienteAsync(Datos.NegocioId, 999, null)).Should().ThrowAsync<NotFoundException>();
    }
}
