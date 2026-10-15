using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Application.Mapping;
using Adif.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Adif.Application.Services;

public sealed class VentaService(
    IVentaRepository ventas,
    IProductoRepository productos,
    ISocioRepository socios,
    IAccesoNegocio acceso,
    IUnitOfWork uow,
    TimeProvider reloj,
    ILogger<VentaService> logger) : IVentaService
{
    public async Task<VentaDto> RegistrarAsync(string uid, RegistrarVentaDto dto, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, dto.NegocioId, ct);

        if (dto.SocioId > Venta.ConsumidorFinal && await socios.ObtenerClienteAsync(dto.NegocioId, dto.SocioId, ct) is null)
            throw new NotFoundException($"No existe el cliente {dto.SocioId}.");

        var catalogo = await productos.ObtenerVariosAsync(dto.NegocioId, dto.Lineas.Select(l => l.ProductoId).Distinct(), ct);
        var faltantes = dto.Lineas.Select(l => l.ProductoId).Where(id => !catalogo.ContainsKey(id)).Distinct().ToList();
        if (faltantes.Count > 0) throw new NotFoundException($"No existen los productos: {string.Join(", ", faltantes)}.");

        var lineas = dto.Lineas.Select(l => new LineaVenta(catalogo[l.ProductoId], l.Cantidad, l.PrecioMayoreo)).ToList();
        var venta = Venta.Registrar(dto.NegocioId, dto.SocioId, reloj.GetLocalNow().DateTime, lineas, dto.EsCredito, dto.AplicaIsv, dto.Metodo, dto.Comentario, dto.DiasCredito);

        // Venta, renglones y stock se confirman en un solo SaveChanges (transacción implícita).
        await ventas.AgregarAsync(venta, ct);
        await uow.GuardarCambiosAsync(ct);

        logger.LogInformation("Venta {VentaId} registrada en negocio {NegocioId} por {Total} (crédito: {EsCredito})", venta.Id, venta.NegocioId, venta.Total, venta.EsCredito);
        return venta.ADto();
    }

    public async Task<PagedResult<VentaDto>> ListarAsync(string uid, int negocioId, DateTime? desde, DateTime? hasta, int pagina, int tamano, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);

        var inicio = desde ?? DateTime.MinValue;
        var fin = hasta ?? DateTime.MaxValue;
        var (p, t) = Paginacion.Normalizar(pagina, tamano);
        var resultado = await ventas.PaginarAsync(v => v.NegocioId == negocioId && v.Fecha >= inicio && v.Fecha <= fin, v => v.Fecha, true, p, t, ct);
        return resultado.Map(v => v.ADto());
    }

    public async Task<VentaDto> ObtenerAsync(string uid, int negocioId, int ventaId, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);
        return (await Cargar(negocioId, ventaId, ct)).ADto();
    }

    public async Task<VentaDto> AnularAsync(string uid, int negocioId, int ventaId, CancellationToken ct = default)
    {
        await acceso.AsegurarPropietarioAsync(uid, negocioId, ct);
        var venta = await Cargar(negocioId, ventaId, ct);
        var afectados = await productos.ObtenerVariosAsync(negocioId, venta.Detalles.Select(d => d.ProductoId).Distinct(), ct);

        venta.Anular(afectados);
        await uow.GuardarCambiosAsync(ct);

        logger.LogInformation("Venta {VentaId} anulada en negocio {NegocioId}", venta.Id, negocioId);
        return venta.ADto();
    }

    private async Task<Venta> Cargar(int negocioId, int ventaId, CancellationToken ct) => await ventas.ObtenerConDetallesAsync(negocioId, ventaId, ct) ?? throw new NotFoundException($"No existe la venta {ventaId}.");
}
