using Adif.Application.Abstractions.Persistence;
using Adif.Application.Common.Exceptions;

namespace Adif.Application.Services;

public sealed class AccesoNegocio(INegocioRepository negocios) : IAccesoNegocio
{
    public async Task AsegurarPropietarioAsync(string uid, int negocioId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(uid) || !await negocios.EsPropietarioAsync(negocioId, uid, ct))
            throw new ForbiddenException("No tienes acceso a ese negocio.");
    }
}
