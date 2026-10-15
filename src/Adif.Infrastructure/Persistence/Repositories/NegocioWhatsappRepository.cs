using Adif.Application.Abstractions.Persistence;
using Adif.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Adif.Infrastructure.Persistence.Repositories;

public sealed class NegocioWhatsappRepository(AdifDbContext db) : Repository<NegocioWhatsapp>(db), INegocioWhatsappRepository
{
    public Task<NegocioWhatsapp?> ObtenerActivoPorPhoneIdAsync(string phoneNumberId, CancellationToken ct = default) => Set.AsNoTracking().FirstOrDefaultAsync(n => n.PhoneNumberId == phoneNumberId && n.Activo, ct);
}
