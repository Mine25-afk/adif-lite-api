using Adif.Application.Dtos;

namespace Adif.Application.Abstractions.Services;

// Estado de la conversación de cada operador con el bot de fiado.
public interface ISesionBotStore
{
    SesionBot Obtener(int negocioId, string telefono);
    void Reiniciar(int negocioId, string telefono);
}
