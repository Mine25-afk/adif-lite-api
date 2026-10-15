// Venta de contado o a crédito (fiado) con sus reglas de totales, saldo y anulación.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Adif.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Adif.Domain.Entities;

[Table("ventas")]
[Index(nameof(NegocioId))]
[Index(nameof(NegocioId), nameof(SocioId))]
public class Venta : IPerteneceANegocio
{
    public const decimal TasaIsv = 0.15m;
    public const int ConsumidorFinal = 0;

    [Key, Column("id")] public int Id { get; set; }
    [Column("negocio_id")] public int NegocioId { get; set; }
    [Column("socios_id")] public int SocioId { get; set; }
    [Column("fecha", TypeName = "timestamp without time zone")] public DateTime Fecha { get; set; } = DateTime.Now;
    [Column("fecha_vence", TypeName = "timestamp without time zone")] public DateTime FechaVence { get; set; } = Fechas.SinFecha;
    [Column("total", TypeName = "numeric(12,2)")] public decimal Total { get; set; }
    [Column("es_credito")] public bool EsCredito { get; set; }
    [Column("saldo", TypeName = "numeric(12,2)")] public decimal Saldo { get; set; }
    [Column("metodo")] public MetodoPago Metodo { get; set; } = MetodoPago.Efectivo;
    [Column("comentario")] public string Comentario { get; set; } = string.Empty;
    [Column("estado")] public EstadoVenta Estado { get; set; } = EstadoVenta.Activa;
    [Column("isv", TypeName = "numeric(12,2)")] public decimal Isv { get; set; }

    [InverseProperty(nameof(VentaDetalle.Venta))]
    public virtual ICollection<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();

    [NotMapped] public decimal Subtotal => Total - Isv;
    [NotMapped] public bool TieneAbonos => EsCredito && Saldo < Total;

    public static Venta Registrar(int negocioId, int socioId, DateTime fecha, IReadOnlyCollection<LineaVenta> lineas,
        bool esCredito, bool aplicaIsv, MetodoPago metodo, string? comentario, int diasCredito)
    {
        if (lineas.Count == 0) throw new DomainException("La venta debe tener al menos un producto.");
        if (esCredito && socioId <= ConsumidorFinal) throw new DomainException("Una venta a crédito necesita un cliente.");
        if (diasCredito < 0) throw new DomainException("Los días de crédito no pueden ser negativos.");

        var venta = new Venta
        {
            NegocioId = negocioId,
            SocioId = socioId,
            Fecha = fecha,
            EsCredito = esCredito,
            Metodo = metodo,
            Comentario = comentario?.Trim() ?? string.Empty,
            FechaVence = esCredito && diasCredito > 0 ? fecha.Date.AddDays(diasCredito) : Fechas.SinFecha
        };

        foreach (var linea in lineas)
        {
            if (linea.Producto.NegocioId != negocioId) throw new DomainException($"El producto {linea.Producto.Id} no pertenece a este negocio.");
            linea.Producto.DescontarStock(linea.Cantidad);
            venta.Detalles.Add(new VentaDetalle
            {
                NegocioId = negocioId,
                ProductoId = linea.Producto.Id,
                Precio = linea.Producto.PrecioPara(linea.PrecioMayoreo),
                Costo = linea.Producto.Costo,
                Cantidad = linea.Cantidad
            });
        }

        var subtotal = venta.Detalles.Sum(d => d.Importe);
        venta.Isv = aplicaIsv ? Dinero.Redondear(subtotal * TasaIsv) : 0m;
        venta.Total = subtotal + venta.Isv;
        venta.Saldo = esCredito ? venta.Total : 0m;
        return venta;
    }

    public static Venta CargoFiado(int negocioId, int socioId, decimal monto, string? descripcion, DateTime fecha)
    {
        if (monto <= 0) throw new DomainException("El monto debe ser mayor a cero.");
        if (socioId <= ConsumidorFinal) throw new DomainException("Un crédito necesita un cliente.");

        return new Venta
        {
            NegocioId = negocioId,
            SocioId = socioId,
            Fecha = fecha,
            EsCredito = true,
            Total = Dinero.Redondear(monto),
            Saldo = Dinero.Redondear(monto),
            Comentario = string.IsNullOrWhiteSpace(descripcion) ? "Crédito por WhatsApp" : descripcion.Trim()
        };
    }

    public decimal AplicarAbono(decimal monto)
    {
        if (monto <= 0) throw new DomainException("El abono debe ser mayor a cero.");
        if (!EsCredito || Estado != EstadoVenta.Activa) throw new DomainException("Solo se abona a créditos activos.");

        var aplicado = Math.Min(Saldo, monto);
        Saldo -= aplicado;
        return aplicado;
    }

    public void Anular(IReadOnlyDictionary<int, Producto> productos)
    {
        if (Estado == EstadoVenta.Anulada) throw new DomainException("La venta ya está anulada.");
        if (TieneAbonos) throw new DomainException("No se puede anular un crédito que ya tiene abonos.");

        foreach (var detalle in Detalles)
            if (productos.TryGetValue(detalle.ProductoId, out var producto)) producto.ReponerStock(detalle.Cantidad);

        Estado = EstadoVenta.Anulada;
        Saldo = 0m;
    }
}
