namespace Adif.Application.Abstractions.Services;

// Evita procesar dos veces el mismo mensaje cuando el proveedor reintenta el webhook.
public interface IDeduplicadorMensajes
{
    bool EsNuevo(string mensajeId);
}
