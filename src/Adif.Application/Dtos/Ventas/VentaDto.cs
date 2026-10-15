// Venta registrada con sus totales.
using Adif.Domain.Common;
using Adif.Domain.Entities;

namespace Adif.Application.Dtos;

public sealed class VentaDto
{
    public int Id { get; init; }
    public int NegocioId { get; init; }
    public int SocioId { get; init; }
    public DateTime Fecha { get; init; } = DateTime.Now;
    public DateTime FechaVence { get; init; } = Fechas.SinFecha;
    public bool EsCredito { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Isv { get; init; }
    public decimal Total { get; init; }
    public decimal Saldo { get; init; }
    public MetodoPago Metodo { get; init; } = MetodoPago.Efectivo;
    public EstadoVenta Estado { get; init; } = EstadoVenta.Activa;
    public string Comentario { get; init; } = string.Empty;
    public IReadOnlyList<VentaDetalleDto> Detalles { get; init; } = [];
}
