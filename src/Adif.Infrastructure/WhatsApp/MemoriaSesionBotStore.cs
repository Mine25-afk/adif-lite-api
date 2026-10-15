using System.Collections.Concurrent;
using Adif.Application.Abstractions.Services;
using Adif.Application.Dtos;

namespace Adif.Infrastructure.WhatsApp;

// En memoria del proceso: con más de una réplica habría que moverlo a Redis.
public sealed class MemoriaSesionBotStore(TimeProvider reloj) : ISesionBotStore
{
    private static readonly TimeSpan Expiracion = TimeSpan.FromMinutes(30);
    private readonly ConcurrentDictionary<string, SesionBot> _sesiones = new();

    public SesionBot Obtener(int negocioId, string telefono)
    {
        var ahora = reloj.GetLocalNow().DateTime;
        var sesion = _sesiones.AddOrUpdate(Clave(negocioId, telefono), _ => new SesionBot { Tocada = ahora }, (_, actual) => ahora - actual.Tocada > Expiracion ? new SesionBot { Tocada = ahora } : actual);
        sesion.Tocada = ahora;
        return sesion;
    }

    public void Reiniciar(int negocioId, string telefono) => _sesiones[Clave(negocioId, telefono)] = new SesionBot { Tocada = reloj.GetLocalNow().DateTime };

    private static string Clave(int negocioId, string telefono) => $"{negocioId}:{telefono}";
}
