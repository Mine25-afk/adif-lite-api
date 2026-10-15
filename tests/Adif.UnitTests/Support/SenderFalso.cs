using Adif.Application.Abstractions.Services;
using Adif.Application.Dtos;

namespace Adif.UnitTests.Support;

// Registra lo que el bot "envía" para poder afirmarlo sin red.
public sealed class SenderFalso : IWhatsAppSender
{
    public List<EnvioWhatsapp> Enviados { get; } = [];
    public List<IReadOnlyList<BotonWhatsapp>> Botones { get; } = [];

    public Task<EnvioWhatsapp> EnviarTextoAsync(CuentaWhatsapp cuenta, string para, string texto, CancellationToken ct = default) => Registrar("text", texto);

    public Task<EnvioWhatsapp> EnviarBotonesAsync(CuentaWhatsapp cuenta, string para, string texto, IReadOnlyList<BotonWhatsapp> botones, CancellationToken ct = default)
    {
        Botones.Add(botones);
        return Registrar("button", texto);
    }

    public Task<EnvioWhatsapp> EnviarListaAsync(CuentaWhatsapp cuenta, string para, string texto, string textoBoton, IReadOnlyList<OpcionListaWhatsapp> opciones, CancellationToken ct = default) => Registrar("list", texto);

    private Task<EnvioWhatsapp> Registrar(string tipo, string texto)
    {
        var envio = new EnvioWhatsapp { Simulado = true, Tipo = tipo, Texto = texto };
        Enviados.Add(envio);
        return Task.FromResult(envio);
    }
}
