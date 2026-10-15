// Renglón de una venta registrada.
namespace Adif.Application.Dtos;

public sealed class VentaDetalleDto
{
    public int ProductoId { get; init; }
    public decimal Cantidad { get; init; }
    public decimal Precio { get; init; }
    public decimal Importe { get; init; }
}
