using Adif.Application.Dtos;

namespace Adif.Application.Abstractions.Services;

// Envío de mensajes de WhatsApp con la cuenta del negocio que se está atendiendo.
public interface IWhatsAppSender
{
    Task<EnvioWhatsapp> EnviarTextoAsync(CuentaWhatsapp cuenta, string para, string texto, CancellationToken ct = default);
    Task<EnvioWhatsapp> EnviarBotonesAsync(CuentaWhatsapp cuenta, string para, string texto, IReadOnlyList<BotonWhatsapp> botones, CancellationToken ct = default);
    Task<EnvioWhatsapp> EnviarListaAsync(CuentaWhatsapp cuenta, string para, string texto, string textoBoton, IReadOnlyList<OpcionListaWhatsapp> opciones, CancellationToken ct = default);
}
