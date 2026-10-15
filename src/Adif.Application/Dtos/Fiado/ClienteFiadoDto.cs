// Cliente con su saldo de fiado actual.
namespace Adif.Application.Dtos;

public sealed class ClienteFiadoDto
{
    public int Id { get; init; }
    public string Nombre { get; init; } = string.Empty;
    public string Telefono { get; init; } = string.Empty;
    public decimal Saldo { get; init; }
    public bool YaExistia { get; init; }
}
