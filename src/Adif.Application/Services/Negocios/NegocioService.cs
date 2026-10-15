using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Mapping;
using Adif.Domain.Entities;

namespace Adif.Application.Services;

public sealed class NegocioService(
    INegocioRepository negocios,
    INegocioWhatsappRepository numeros,
    IRepository<WhatsappOperador> operadores,
    IAccesoNegocio acceso,
    IUnitOfWork uow,
    TimeProvider reloj) : INegocioService
{
    public async Task<NegocioDto> CrearAsync(string uid, CrearNegocioDto dto, CancellationToken ct = default)
    {
        var phoneId = dto.PhoneNumberId.Trim();
        // El phone_number_id resuelve el tenant de cada mensaje entrante: no puede repetirse.
        if (await numeros.ExisteAsync(n => n.PhoneNumberId == phoneId, ct)) throw new ConflictException("Ese phone_number_id ya está registrado en otra empresa.");

        var ahora = reloj.GetLocalNow().DateTime;
        var negocio = new Negocio
        {
            Nombre = dto.Nombre.Trim(),
            Propietario = uid,
            Telefono = dto.Telefono.Trim(),
            Direccion = dto.Direccion.Trim(),
            Rtn = dto.Rtn.Trim(),
            CreadoEn = ahora
        };
        negocio.Numeros.Add(new NegocioWhatsapp
        {
            PhoneNumberId = phoneId,
            DisplayNumber = dto.DisplayNumber.Trim(),
            WabaId = dto.WabaId.Trim(),
            WebhookSecret = dto.WebhookSecret.Trim(),
            KapsoApiKey = dto.KapsoApiKey.Trim(),
            CreadoEn = ahora
        });
        await negocios.AgregarAsync(negocio, ct);

        var telefonoOperador = WhatsappOperador.NormalizarTelefono(dto.OperadorTelefono);
        if (telefonoOperador.Length > 0)
            await operadores.AgregarAsync(new WhatsappOperador { Negocio = negocio, Telefono = telefonoOperador, Nombre = dto.OperadorNombre.Trim() }, ct);

        await uow.GuardarCambiosAsync(ct);
        return negocio.ADto();
    }

    public async Task<IReadOnlyList<NegocioDto>> ListarMiosAsync(string uid, CancellationToken ct = default) => (await negocios.ListarPorPropietarioAsync(uid, ct)).Select(n => n.ADto()).ToList();

    public async Task<bool> AgregarOperadorAsync(string uid, AgregarOperadorDto dto, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, dto.NegocioId, ct);

        var telefono = WhatsappOperador.NormalizarTelefono(dto.Telefono);
        if (await operadores.ExisteAsync(o => o.NegocioId == dto.NegocioId && o.Telefono == telefono, ct)) return false;

        await operadores.AgregarAsync(new WhatsappOperador { NegocioId = dto.NegocioId, Telefono = telefono, Nombre = dto.Nombre.Trim() }, ct);
        await uow.GuardarCambiosAsync(ct);
        return true;
    }
}
