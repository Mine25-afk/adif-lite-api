using Adif.Application.Dtos;

namespace Adif.Application.Services;

// Fiado de un negocio ya resuelto: el que llama (bot o herramienta) garantiza el tenant.
public interface IFiadoService
{
    Task<ClienteFiadoDto> CrearClienteAsync(int negocioId, string nombre, string? telefono, CancellationToken ct = default);
    Task<IReadOnlyList<ClienteFiadoDto>> BuscarClientesAsync(int negocioId, string? texto, CancellationToken ct = default);
    Task<ClienteFiadoDto> ObtenerSaldoAsync(int negocioId, int socioId, CancellationToken ct = default);
    Task<int> ResolverClienteAsync(int negocioId, int clienteId, string? nombre, CancellationToken ct = default);
    Task<ClienteFiadoDto> AgregarCargoAsync(int negocioId, int socioId, decimal monto, string? descripcion, CancellationToken ct = default);
    Task<AbonoResultadoDto> RegistrarAbonoAsync(int negocioId, int socioId, decimal monto, CancellationToken ct = default);
    Task<IReadOnlyList<DeudorDto>> ListarDeudoresAsync(int negocioId, CancellationToken ct = default);
}
