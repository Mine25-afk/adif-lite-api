using Adif.Application.Dtos;
using Adif.Domain.Entities;

namespace Adif.Application.Mapping;

// Conversión entidad → DTO en un solo lugar; las credenciales nunca salen en un DTO.
public static class Mapeos
{
    public static ProductoDto ADto(this Producto p) => new()
    {
        Id = p.Id,
        NegocioId = p.NegocioId,
        Nombre = p.Nombre,
        Precio = p.Precio,
        PrecioMayoreo = p.PrecioMayoreo,
        Costo = p.Costo,
        Stock = p.Stock,
        StockMinimo = p.StockMinimo,
        StockBajo = p.StockBajo,
        Unidad = p.Unidad,
        CodigoBarra = p.CodigoBarra,
        CategoriaId = p.CategoriaId,
        Activo = p.Activo,
        TieneImagen = p.ImagenBlob.Length > 0
    };

    public static VentaDto ADto(this Venta v) => new()
    {
        Id = v.Id,
        NegocioId = v.NegocioId,
        SocioId = v.SocioId,
        Fecha = v.Fecha,
        FechaVence = v.FechaVence,
        EsCredito = v.EsCredito,
        Subtotal = v.Subtotal,
        Isv = v.Isv,
        Total = v.Total,
        Saldo = v.Saldo,
        Metodo = v.Metodo,
        Estado = v.Estado,
        Comentario = v.Comentario,
        Detalles = v.Detalles.Select(d => new VentaDetalleDto { ProductoId = d.ProductoId, Cantidad = d.Cantidad, Precio = d.Precio, Importe = d.Importe }).ToList()
    };

    public static NegocioDto ADto(this Negocio n) => new()
    {
        Id = n.Id,
        Nombre = n.Nombre,
        Telefono = n.Telefono,
        Direccion = n.Direccion,
        Rtn = n.Rtn,
        Activo = n.Activo,
        Numeros = n.Numeros.Select(w => new NumeroWhatsappDto
        {
            Id = w.Id,
            PhoneNumberId = w.PhoneNumberId,
            DisplayNumber = w.DisplayNumber,
            WabaId = w.WabaId,
            Activo = w.Activo,
            FirmaConfigurada = w.WebhookSecret.Length > 0
        }).ToList()
    };

    public static DispositivoDto ADto(this LicenciaDispositivo d, DateTime hoy) => new()
    {
        Usuario = d.Usuario,
        Telefono = d.Telefono,
        Bloqueado = d.Bloqueado,
        Mensualidad = d.Mensualidad,
        Cantidad = d.Cantidad,
        Vigente = d.Vigente(hoy)
    };

    public static ClienteFiadoDto ADto(this Socio s, decimal saldo, bool yaExistia = false) => new() { Id = s.Id, Nombre = s.Nombre, Telefono = s.Telefono, Saldo = saldo, YaExistia = yaExistia };
}
