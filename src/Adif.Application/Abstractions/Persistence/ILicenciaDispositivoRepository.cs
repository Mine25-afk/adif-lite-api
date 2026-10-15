using Adif.Domain.Entities;

namespace Adif.Application.Abstractions.Persistence;

public interface ILicenciaDispositivoRepository : IRepository<LicenciaDispositivo>
{
    Task<LicenciaDispositivo?> ObtenerAsync(string usuario, string telefono, CancellationToken ct = default);
    Task<IReadOnlyList<LicenciaDispositivo>> ListarPorUidAsync(string uid, CancellationToken ct = default);
}
