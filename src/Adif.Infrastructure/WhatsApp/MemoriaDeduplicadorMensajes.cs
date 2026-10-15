using System.Collections.Concurrent;
using Adif.Application.Abstractions.Services;

namespace Adif.Infrastructure.WhatsApp;

// Ventana acotada de ids recientes; los reintentos de Kapso llegan en segundos.
public sealed class MemoriaDeduplicadorMensajes : IDeduplicadorMensajes
{
    private const int Capacidad = 1000;
    private readonly ConcurrentDictionary<string, byte> _vistos = new();
    private readonly ConcurrentQueue<string> _orden = new();

    public bool EsNuevo(string mensajeId)
    {
        if (!_vistos.TryAdd(mensajeId, 0)) return false;

        _orden.Enqueue(mensajeId);
        while (_orden.Count > Capacidad && _orden.TryDequeue(out var viejo)) _vistos.TryRemove(viejo, out _);
        return true;
    }
}
