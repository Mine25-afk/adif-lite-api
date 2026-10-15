namespace Adif.Application.Services;

// Aislamiento multi-tenant: un usuario solo opera los negocios que registró.
public interface IAccesoNegocio
{
    Task AsegurarPropietarioAsync(string uid, int negocioId, CancellationToken ct = default);
}
