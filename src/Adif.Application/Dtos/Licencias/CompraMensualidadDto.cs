// Compra de meses de licencia.
namespace Adif.Application.Dtos;

public sealed class CompraMensualidadDto
{
    public string Telefono { get; set; } = string.Empty;
    public int Meses { get; set; }
}
