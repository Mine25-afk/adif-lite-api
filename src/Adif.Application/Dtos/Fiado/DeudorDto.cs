// Cliente con deuda pendiente.
namespace Adif.Application.Dtos;

public sealed class DeudorDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public decimal Saldo { get; init; }
}
