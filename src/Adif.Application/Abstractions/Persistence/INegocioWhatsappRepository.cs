using Adif.Domain.Entities;

namespace Adif.Application.Abstractions.Persistence;

public interface INegocioWhatsappRepository : IRepository<NegocioWhatsapp>
{
    Task<NegocioWhatsapp?> ObtenerActivoPorPhoneIdAsync(string phoneNumberId, CancellationToken ct = default);
}
