// Producto del inventario tal como lo ve la app.
namespace Adif.Application.Dtos;

public sealed class ProductoDto
{
    public int Id { get; init; }
    public int NegocioId { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public decimal Precio { get; init; }
    public decimal PrecioMayoreo { get; init; }
    public decimal Costo { get; init; }
    public decimal Stock { get; init; }
    public decimal StockMinimo { get; init; }
    public bool StockBajo { get; init; }
    public string Unidad { get; init; } = string.Empty;
    public string CodigoBarra { get; init; } = string.Empty;
    public int CategoriaId { get; init; }
    public bool Activo { get; init; }
    public bool TieneImagen { get; init; }
}
