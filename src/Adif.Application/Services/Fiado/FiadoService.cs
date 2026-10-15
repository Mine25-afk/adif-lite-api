using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Mapping;
using Adif.Domain.Entities;
using Adif.Domain.Fiado;

namespace Adif.Application.Services;

// Una deuda es una venta a crédito con saldo; un pago es un abono repartido FIFO entre ellas.
public sealed class FiadoService(
    ISocioRepository socios,
    IVentaRepository ventas,
    IRepository<Abono> abonos,
    IUnitOfWork uow,
    TimeProvider reloj) : IFiadoService
{
    private const int MaximoResultadosBusqueda = 10;

    public async Task<ClienteFiadoDto> CrearClienteAsync(int negocioId, string nombre, string? telefono, CancellationToken ct = default)
    {
        var existente = await socios.ObtenerClientePorNombreAsync(negocioId, nombre.Trim(), ct);
        if (existente is not null) return existente.ADto(await ventas.CalcularSaldoClienteAsync(negocioId, existente.Id, ct), yaExistia: true);

        var socio = Socio.NuevoCliente(negocioId, nombre, telefono);
        await socios.AgregarAsync(socio, ct);
        await uow.GuardarCambiosAsync(ct);
        return socio.ADto(0m);
    }

    public Task<IReadOnlyList<ClienteFiadoDto>> BuscarClientesAsync(int negocioId, string? texto, CancellationToken ct = default) => socios.BuscarClientesConSaldoAsync(negocioId, (texto ?? string.Empty).Trim(), MaximoResultadosBusqueda, ct);

    public async Task<ClienteFiadoDto> ObtenerSaldoAsync(int negocioId, int socioId, CancellationToken ct = default)
    {
        var socio = await CargarCliente(negocioId, socioId, ct);
        return socio.ADto(await ventas.CalcularSaldoClienteAsync(negocioId, socioId, ct));
    }

    public async Task<int> ResolverClienteAsync(int negocioId, int clienteId, string? nombre, CancellationToken ct = default)
    {
        if (clienteId > 0) return (await CargarCliente(negocioId, clienteId, ct)).Id;

        var coincidencias = await BuscarClientesAsync(negocioId, nombre, ct);
        return coincidencias.Count switch
        {
            0 => throw new NotFoundException($"No encontré al cliente '{nombre}'."),
            1 => coincidencias[0].Id,
            _ => throw new ConflictException($"Hay {coincidencias.Count} clientes que coinciden con '{nombre}'. Sé más específico o usa el id.")
        };
    }

    public async Task<ClienteFiadoDto> AgregarCargoAsync(int negocioId, int socioId, decimal monto, string? descripcion, CancellationToken ct = default)
    {
        var socio = await CargarCliente(negocioId, socioId, ct);
        var saldoPrevio = await ventas.CalcularSaldoClienteAsync(negocioId, socioId, ct);

        var cargo = Venta.CargoFiado(negocioId, socioId, monto, descripcion, reloj.GetLocalNow().DateTime);
        await ventas.AgregarAsync(cargo, ct);
        await uow.GuardarCambiosAsync(ct);
        return socio.ADto(saldoPrevio + cargo.Saldo);
    }

    public async Task<AbonoResultadoDto> RegistrarAbonoAsync(int negocioId, int socioId, decimal monto, CancellationToken ct = default)
    {
        await CargarCliente(negocioId, socioId, ct);
        var abiertos = await ventas.ListarCreditosAbiertosAsync(negocioId, socioId, ct);

        var resultado = DistribucionAbono.Aplicar(abiertos, monto, reloj.GetLocalNow().DateTime, "Abono por WhatsApp");
        foreach (var abono in resultado.Abonos) await abonos.AgregarAsync(abono, ct);
        await uow.GuardarCambiosAsync(ct);

        var mensaje = resultado.Aplicado <= 0
            ? "El cliente no tenía deuda pendiente; no se aplicó ningún abono."
            : resultado.Sobrante > 0 ? "Abono aplicado. El monto superaba la deuda; solo se aplicó lo adeudado." : "Abono aplicado.";

        return new AbonoResultadoDto { Aplicado = resultado.Aplicado, Sobrante = resultado.Sobrante, SaldoRestante = abiertos.Sum(v => v.Saldo), Mensaje = mensaje };
    }

    public Task<IReadOnlyList<DeudorDto>> ListarDeudoresAsync(int negocioId, CancellationToken ct = default) => ventas.ListarDeudoresAsync(negocioId, ct);

    private async Task<Socio> CargarCliente(int negocioId, int socioId, CancellationToken ct) => await socios.ObtenerClienteAsync(negocioId, socioId, ct) ?? throw new NotFoundException($"No existe el cliente {socioId}.");
}
