using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;
using Adif.Domain.Entities;

namespace Adif.Application.Services;

public sealed class WhatsappContextService(INegocioWhatsappRepository numeros, IRepository<WhatsappOperador> operadores) : IWhatsappContextService
{
    public Task<NegocioWhatsapp?> ObtenerNumeroAsync(string phoneNumberId, CancellationToken ct = default) => string.IsNullOrWhiteSpace(phoneNumberId) ? Task.FromResult<NegocioWhatsapp?>(null) : numeros.ObtenerActivoPorPhoneIdAsync(phoneNumberId.Trim(), ct);

    public async Task<bool> EstaAutorizadoAsync(int negocioId, string telefono, CancellationToken ct = default)
    {
        var normalizado = WhatsappOperador.NormalizarTelefono(telefono);
        return normalizado.Length > 0 && await operadores.ExisteAsync(o => o.NegocioId == negocioId && o.Activo && o.Telefono == normalizado, ct);
    }

    public async Task<int> ResolverOperadorAsync(string phoneNumberId, string telefono, CancellationToken ct = default)
    {
        var numero = await ObtenerNumeroAsync(phoneNumberId, ct) ?? throw new NotFoundException("Número de WhatsApp no registrado.");
        if (!await EstaAutorizadoAsync(numero.NegocioId, telefono, ct)) throw new ForbiddenException("Remitente no autorizado para operar el fiado.");
        return numero.NegocioId;
    }
}
