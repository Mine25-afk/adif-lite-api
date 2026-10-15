using Adif.Domain.Entities;

namespace Adif.Application.Services;

// Resuelve el tenant de un mensaje de WhatsApp y autoriza al remitente.
public interface IWhatsappContextService
{
    Task<NegocioWhatsapp?> ObtenerNumeroAsync(string phoneNumberId, CancellationToken ct = default);
    Task<bool> EstaAutorizadoAsync(int negocioId, string telefono, CancellationToken ct = default);
    Task<int> ResolverOperadorAsync(string phoneNumberId, string telefono, CancellationToken ct = default);
}
