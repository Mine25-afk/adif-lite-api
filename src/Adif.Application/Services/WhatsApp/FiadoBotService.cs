using System.Globalization;
using Adif.Application.Abstractions.Services;
using Adif.Application.Common.Exceptions;
using Adif.Application.Dtos;
using Adif.Domain.Common;
using static Adif.Application.Services.PasosBot;

namespace Adif.Application.Services;

public sealed class FiadoBotService(IFiadoService fiado, IWhatsappContextService contexto, IWhatsAppSender sender, ISesionBotStore sesiones) : IFiadoBotService
{
    private static readonly string[] PalabrasMenu = ["cancelar", "menu", "menú", "hola", "inicio", "0"];

    private sealed record Conversacion(int NegocioId, CuentaWhatsapp Cuenta, string From, SesionBot Sesion, CancellationToken Ct);

    public async Task<IReadOnlyList<EnvioWhatsapp>> ProcesarAsync(NegocioDelMensaje negocio, MensajeEntrante mensaje, CancellationToken ct = default)
    {
        var enviados = new List<EnvioWhatsapp>();
        var c = new Conversacion(negocio.NegocioId, negocio.Cuenta, mensaje.From, sesiones.Obtener(negocio.NegocioId, mensaje.From), ct);

        if (!await contexto.EstaAutorizadoAsync(c.NegocioId, c.From, ct))
        {
            enviados.Add(await Texto(c, "Hola 👋 Este es el asistente del negocio. Tu número no está autorizado para operar el crédito."));
            return enviados;
        }

        var texto = mensaje.Texto.Trim();
        var reply = mensaje.RespuestaId;

        if (reply is Cancelar or Menu || PalabrasMenu.Contains(texto.ToLowerInvariant()))
        {
            sesiones.Reiniciar(c.NegocioId, c.From);
            enviados.Add(await EnviarMenu(c));
            return enviados;
        }

        try
        {
            enviados.AddRange(c.Sesion.Paso switch
            {
                Inicio => await PasoMenu(c, reply),
                Nombre => await PasoNombre(c, texto),
                Elegir => await PasoElegir(c, reply),
                Monto => await PasoMonto(c, texto),
                OfrecerCrear => await PasoOfrecerCrear(c, reply),
                NuevoNombre => await PasoNuevoNombre(c, texto),
                NuevoTelefono => await PasoNuevoTelefono(c, texto, reply),
                _ => await Reiniciar(c)
            });
        }
        catch (Exception ex) when (ex is DomainException or AppException)
        {
            // Errores de negocio se le explican al operador y la conversación vuelve al inicio.
            sesiones.Reiniciar(c.NegocioId, c.From);
            enviados.Add(await Botones(c, $"⚠️ {ex.Message}", (Menu, "📋 Menú")));
        }

        return enviados;
    }

    private async Task<List<EnvioWhatsapp>> PasoMenu(Conversacion c, string reply)
    {
        if (reply.StartsWith(PrefijoFiar) || reply.StartsWith(PrefijoAbonar))
        {
            var accion = reply.StartsWith(PrefijoFiar) ? AccionFiado : AccionAbono;
            if (!int.TryParse(reply[(reply.IndexOf('_') + 1)..], out var clienteId)) return [await EnviarMenu(c)];

            var cliente = await fiado.ObtenerSaldoAsync(c.NegocioId, clienteId, c.Ct);
            c.Sesion.Accion = accion;
            return await ClienteResuelto(c, cliente.Id, cliente.Nombre, cliente.Saldo);
        }

        switch (reply)
        {
            case MenuFiado: return [await IniciarAccion(c, AccionFiado, "💰 *Anotar crédito*\n¿Nombre del cliente?")];
            case MenuAbono: return [await IniciarAccion(c, AccionAbono, "✅ *Registrar abono*\n¿Nombre del cliente?")];
            case MenuSaldo: return [await IniciarAccion(c, AccionSaldo, "🔍 *Consultar saldo*\n¿Nombre del cliente?")];
            case MenuNuevo:
                c.Sesion.Paso = NuevoNombre;
                return [await Texto(c, "➕ *Nuevo cliente*\n¿Nombre completo?")];
            case MenuDeudores:
                var deudores = await fiado.ListarDeudoresAsync(c.NegocioId, c.Ct);
                if (deudores.Count == 0) return [await Botones(c, "🎉 Nadie debe en este momento.", (MenuFiado, "💰 Anotar crédito"), (Menu, "📋 Menú"))];

                var lineas = string.Join("\n", deudores.Select(d => $"• {d.Nombre} — *L{d.Saldo:N2}*"));
                return [await Botones(c, $"📋 *Deudores:*\n{lineas}\n\nTotal en créditos: *L{deudores.Sum(d => d.Saldo):N2}*", (MenuAbono, "✅ Registrar abono"), (MenuFiado, "💰 Anotar crédito"), (Menu, "📋 Menú"))];
            default:
                return [await EnviarMenu(c)];
        }
    }

    private async Task<List<EnvioWhatsapp>> PasoNombre(Conversacion c, string texto)
    {
        if (texto.Length == 0) return [await Texto(c, "Escribe el nombre del cliente, o *cancelar* para volver al menú.")];

        var lista = await fiado.BuscarClientesAsync(c.NegocioId, texto, c.Ct);
        if (lista.Count == 1) return await ClienteResuelto(c, lista[0].Id, lista[0].Nombre, lista[0].Saldo);

        if (lista.Count == 0)
        {
            if (c.Sesion.Accion != AccionFiado) return [await Texto(c, $"No encontré a *{texto}*. Intenta con otro nombre o escribe *cancelar*.")];

            c.Sesion.NombrePendiente = texto;
            c.Sesion.Paso = OfrecerCrear;
            return [await Botones(c, $"No encontré a *{texto}*. ¿Lo creo como cliente nuevo?", (CrearSi, "Sí, crearlo"), (Cancelar, "Cancelar"))];
        }

        c.Sesion.Paso = Elegir;
        var opciones = lista.Select(x => new OpcionListaWhatsapp($"{PrefijoCliente}{x.Id}", x.Nombre, $"Debe L{x.Saldo:N2}")).ToList();
        return [await sender.EnviarListaAsync(c.Cuenta, c.From, $"Encontré {lista.Count} clientes con \"{texto}\". ¿Cuál es?", "Elegir cliente", opciones, c.Ct)];
    }

    private async Task<List<EnvioWhatsapp>> PasoElegir(Conversacion c, string reply)
    {
        if (!reply.StartsWith(PrefijoCliente) || !int.TryParse(reply[PrefijoCliente.Length..], out var clienteId))
            return [await Texto(c, "Elige un cliente de la lista, o escribe *cancelar*.")];

        var cliente = await fiado.ObtenerSaldoAsync(c.NegocioId, clienteId, c.Ct);
        return await ClienteResuelto(c, cliente.Id, cliente.Nombre, cliente.Saldo);
    }

    private async Task<List<EnvioWhatsapp>> ClienteResuelto(Conversacion c, int clienteId, string nombre, decimal saldo)
    {
        if (c.Sesion.Accion == AccionSaldo)
        {
            sesiones.Reiniciar(c.NegocioId, c.From);
            return saldo > 0
                ? [await Botones(c, $"🔍 *{nombre}* debe *L{saldo:N2}*.", ($"{PrefijoFiar}{clienteId}", "💰 Dar crédito"), ($"{PrefijoAbonar}{clienteId}", "✅ Abonó"), (Menu, "📋 Menú"))]
                : [await Botones(c, $"🔍 *{nombre}* no tiene deuda pendiente. 🎉", ($"{PrefijoFiar}{clienteId}", "💰 Dar crédito"), (Menu, "📋 Menú"))];
        }

        c.Sesion.ClienteId = clienteId;
        c.Sesion.ClienteNombre = nombre;
        c.Sesion.Paso = Monto;
        var verbo = c.Sesion.Accion == AccionFiado ? "dar de crédito" : "abonar";
        return [await Texto(c, $"*{nombre}* (debe L{saldo:N2}).\n¿Cuánto va a {verbo}? (solo el número, en Lempiras)")];
    }

    private async Task<List<EnvioWhatsapp>> PasoMonto(Conversacion c, string texto)
    {
        if (!TryLeerMonto(texto, out var monto)) return [await Texto(c, "No entendí el monto. Escribe solo el número, por ejemplo: *50*")];
        if (c.Sesion.ClienteId <= 0) return await Reiniciar(c);

        var s = c.Sesion;
        sesiones.Reiniciar(c.NegocioId, c.From);

        if (s.Accion == AccionFiado)
        {
            var cliente = await fiado.AgregarCargoAsync(c.NegocioId, s.ClienteId, monto, "Crédito por WhatsApp", c.Ct);
            return [await Botones(c, $"✅ Anotado: *L{monto:N2}* a *{s.ClienteNombre}*.\nAhora debe *L{cliente.Saldo:N2}*.", (MenuFiado, "💰 Otro crédito"), (MenuDeudores, "📋 Deudores"), (Menu, "📋 Menú"))];
        }

        var abono = await fiado.RegistrarAbonoAsync(c.NegocioId, s.ClienteId, monto, c.Ct);
        return [await Botones(c, $"✅ Abono de *L{abono.Aplicado:N2}* aplicado a *{s.ClienteNombre}*.\nSaldo restante: *L{abono.SaldoRestante:N2}*.", (MenuAbono, "✅ Otro abono"), (MenuDeudores, "📋 Deudores"), (Menu, "📋 Menú"))];
    }

    private async Task<List<EnvioWhatsapp>> PasoOfrecerCrear(Conversacion c, string reply)
    {
        if (reply != CrearSi || string.IsNullOrWhiteSpace(c.Sesion.NombrePendiente))
            return [await Botones(c, "¿Creo al cliente?", (CrearSi, "Sí, crearlo"), (Cancelar, "Cancelar"))];

        var cliente = await fiado.CrearClienteAsync(c.NegocioId, c.Sesion.NombrePendiente, null, c.Ct);
        c.Sesion.ClienteId = cliente.Id;
        c.Sesion.ClienteNombre = cliente.Nombre;
        c.Sesion.NombrePendiente = string.Empty;
        c.Sesion.Paso = Monto;
        return [await Texto(c, $"✅ Cliente *{cliente.Nombre}* creado.\n¿Cuánto va a dar de crédito? (solo el número, en Lempiras)")];
    }

    private async Task<List<EnvioWhatsapp>> PasoNuevoNombre(Conversacion c, string texto)
    {
        if (texto.Length == 0) return [await Texto(c, "Escribe el nombre del cliente, o *cancelar*.")];

        c.Sesion.NombrePendiente = texto;
        c.Sesion.Paso = NuevoTelefono;
        return [await Botones(c, $"¿Teléfono de *{texto}*? Escríbelo, o toca Omitir.", (Omitir, "Omitir"))];
    }

    private async Task<List<EnvioWhatsapp>> PasoNuevoTelefono(Conversacion c, string texto, string reply)
    {
        var telefono = reply == Omitir || texto.Length == 0 ? null : texto;
        var nombre = c.Sesion.NombrePendiente;
        sesiones.Reiniciar(c.NegocioId, c.From);

        var cliente = await fiado.CrearClienteAsync(c.NegocioId, nombre, telefono, c.Ct);
        return [await Botones(c, $"✅ Cliente *{cliente.Nombre}* guardado.", ($"{PrefijoFiar}{cliente.Id}", "💰 Dar crédito"), (MenuNuevo, "➕ Otro cliente"), (Menu, "📋 Menú"))];
    }

    internal static bool TryLeerMonto(string texto, out decimal monto)
    {
        var limpio = texto.ToLowerInvariant().Replace("lps", "").Replace("l.", "").Replace("l", "").Replace(",", "").Trim();
        return decimal.TryParse(limpio, NumberStyles.Number, CultureInfo.InvariantCulture, out monto) && monto > 0;
    }

    private async Task<List<EnvioWhatsapp>> Reiniciar(Conversacion c)
    {
        sesiones.Reiniciar(c.NegocioId, c.From);
        return [await EnviarMenu(c)];
    }

    private Task<EnvioWhatsapp> IniciarAccion(Conversacion c, string accion, string pregunta)
    {
        c.Sesion.Accion = accion;
        c.Sesion.Paso = Nombre;
        return Texto(c, pregunta);
    }

    private Task<EnvioWhatsapp> Texto(Conversacion c, string texto) => sender.EnviarTextoAsync(c.Cuenta, c.From, texto, c.Ct);

    private Task<EnvioWhatsapp> Botones(Conversacion c, string texto, params (string Id, string Titulo)[] botones) => sender.EnviarBotonesAsync(c.Cuenta, c.From, texto, botones.Select(b => new BotonWhatsapp(b.Id, b.Titulo)).ToList(), c.Ct);

    private Task<EnvioWhatsapp> EnviarMenu(Conversacion c) => sender.EnviarListaAsync(c.Cuenta, c.From, "👋 ¿Qué deseas hacer?", "Ver opciones",
    [
        new(MenuFiado, "💰 Anotar crédito", "Apuntar deuda a un cliente"),
        new(MenuAbono, "✅ Registrar abono", "El cliente pagó parte o todo"),
        new(MenuSaldo, "🔍 Saldo de cliente", "Cuánto debe alguien"),
        new(MenuDeudores, "📋 Deudores", "Lista de quienes deben"),
        new(MenuNuevo, "➕ Nuevo cliente", "Registrar un cliente")
    ], c.Ct);
}
