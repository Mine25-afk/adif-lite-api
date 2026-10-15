using Adif.Application.Dtos;

namespace Adif.Application.Services;

// Bot determinístico (sin IA) que opera el fiado con botones y listas de WhatsApp.
public interface IFiadoBotService
{
    Task<IReadOnlyList<EnvioWhatsapp>> ProcesarAsync(NegocioDelMensaje negocio, MensajeEntrante mensaje, CancellationToken ct = default);
}
