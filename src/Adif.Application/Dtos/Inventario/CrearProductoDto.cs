// Alta de un producto en el inventario.
namespace Adif.Application.Dtos;

public sealed class CrearProductoDto
{
    public int NegocioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public decimal PrecioMayoreo { get; set; }
    public decimal Costo { get; set; }
    public decimal Stock { get; set; }
    public decimal StockMinimo { get; set; }
    public string Unidad { get; set; } = "unidad";
    public string CodigoBarra { get; set; } = string.Empty;
    public int CategoriaId { get; set; }
    public string Notas { get; set; } = string.Empty;
}
