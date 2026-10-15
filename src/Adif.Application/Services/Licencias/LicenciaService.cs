using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Mapping;
using Adif.Domain.Entities;

namespace Adif.Application.Services;

public sealed class LicenciaService(
    ILicenciaDispositivoRepository dispositivos,
    IRepository<LicenciaLog> bitacora,
    IRepository<CodigoActivacion> codigos,
    IUnitOfWork uow,
    TimeProvider reloj) : ILicenciaService
{
    public async Task<DispositivoDto> RegistrarAccesoAsync(string uid, RegistroAccesoDto dto, CancellationToken ct = default)
    {
        var ahora = Ahora();
        var dispositivo = await dispositivos.ObtenerAsync(dto.Usuario.Trim(), dto.Telefono.Trim(), ct);

        if (dispositivo is null)
        {
            dispositivo = new LicenciaDispositivo { Usuario = dto.Usuario.Trim(), Telefono = dto.Telefono.Trim(), Uid = uid };
            await dispositivos.AgregarAsync(dispositivo, ct);
        }
        else if (dispositivo.Uid.Length == 0) dispositivo.Uid = uid;
        else if (dispositivo.Uid != uid) throw new ForbiddenException("El dispositivo pertenece a otra cuenta.");

        await bitacora.AgregarAsync(new LicenciaLog { Usuario = dispositivo.Usuario, Telefono = dispositivo.Telefono, Fecha = ahora, Accion = dto.Accion.Trim() }, ct);
        await uow.GuardarCambiosAsync(ct);
        return dispositivo.ADto(ahora);
    }

    public async Task<IReadOnlyList<DispositivoDto>> ListarDispositivosAsync(string uid, CancellationToken ct = default)
    {
        var ahora = Ahora();
        return (await dispositivos.ListarPorUidAsync(uid, ct)).Select(d => d.ADto(ahora)).ToList();
    }

    public async Task<DispositivoDto> CrearCuentaAsync(string uid, CrearCuentaDto dto, CancellationToken ct = default)
    {
        if (await dispositivos.ObtenerAsync(dto.Usuario.Trim(), dto.Telefono.Trim(), ct) is not null) throw new ConflictException("Ese usuario ya está registrado en este dispositivo.");

        var dispositivo = new LicenciaDispositivo { Usuario = dto.Usuario.Trim(), Telefono = dto.Telefono.Trim(), Uid = uid };
        await dispositivos.AgregarAsync(dispositivo, ct);
        await uow.GuardarCambiosAsync(ct);
        return dispositivo.ADto(Ahora());
    }

    public async Task<DispositivoDto> ComprarMensualidadAsync(string uid, CompraMensualidadDto dto, CancellationToken ct = default)
    {
        var ahora = Ahora();
        var dispositivo = await CargarPropio(uid, dto.Telefono, ct);

        // La fecha de vencimiento la calcula el servidor; el cliente solo dice cuántos meses.
        dispositivo.ExtenderMensualidad(dto.Meses, ahora);
        await bitacora.AgregarAsync(new LicenciaLog { Usuario = dispositivo.Usuario, Telefono = dispositivo.Telefono, Fecha = ahora, Accion = $"Mensualidad de {dto.Meses} meses" }, ct);
        await uow.GuardarCambiosAsync(ct);
        return dispositivo.ADto(ahora);
    }

    public async Task<DispositivoDto> CambiarBloqueoAsync(string uid, BloqueoDispositivoDto dto, CancellationToken ct = default)
    {
        var dispositivo = await CargarPropio(uid, dto.Telefono, ct);
        dispositivo.Bloqueado = dto.Bloqueado;
        await uow.GuardarCambiosAsync(ct);
        return dispositivo.ADto(Ahora());
    }

    public async Task CanjearCodigoAsync(ValidarCodigoDto dto, CancellationToken ct = default)
    {
        var codigo = await codigos.ObtenerPorIdAsync(dto.Codigo.Trim(), ct) ?? throw new NotFoundException("El código no es válido.");
        codigo.Canjear();
        await uow.GuardarCambiosAsync(ct);
    }

    private async Task<LicenciaDispositivo> CargarPropio(string uid, string telefono, CancellationToken ct) => (await dispositivos.ListarPorUidAsync(uid, ct)).FirstOrDefault(d => d.Telefono == telefono.Trim()) ?? throw new NotFoundException("No tienes un dispositivo con ese teléfono.");

    private DateTime Ahora() => reloj.GetLocalNow().DateTime;
}
