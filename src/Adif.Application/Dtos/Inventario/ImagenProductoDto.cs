// URL firmada y temporal de la imagen de un producto.
namespace Adif.Application.Dtos;

public sealed class ImagenProductoDto
{
    public int ProductoId { get; init; }
    public string Url { get; init; } = string.Empty;
    public DateTime ExpiraEn { get; init; } = DateTime.Now;
}
