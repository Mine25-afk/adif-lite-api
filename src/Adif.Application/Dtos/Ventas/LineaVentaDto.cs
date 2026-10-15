// Renglón pedido en una venta nueva.
namespace Adif.Application.Dtos;

public sealed class LineaVentaDto
{
    public int ProductoId { get; set; }
    public decimal Cantidad { get; set; }
    public bool PrecioMayoreo { get; set; }
}
