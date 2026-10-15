using Adif.Application.Dtos;

namespace Adif.Infrastructure.WhatsApp;

// Cuerpos del Cloud API de WhatsApp respetando sus límites (3 botones, 10 filas, títulos cortos).
public static class KapsoPayloads
{
    public const int MaximoBotones = 3;
    public const int MaximoFilas = 10;

    public static object Texto(string para, string texto) => new { messaging_product = "whatsapp", to = para, type = "text", text = new { body = texto } };

    public static object Botones(string para, string texto, IEnumerable<BotonWhatsapp> botones) => new
    {
        messaging_product = "whatsapp",
        to = para,
        type = "interactive",
        interactive = new
        {
            type = "button",
            body = new { text = texto },
            action = new { buttons = botones.Take(MaximoBotones).Select(b => new { type = "reply", reply = new { id = b.Id, title = Recortar(b.Titulo, 20) } }).ToArray() }
        }
    };

    public static object Lista(string para, string texto, string textoBoton, IEnumerable<OpcionListaWhatsapp> opciones) => new
    {
        messaging_product = "whatsapp",
        to = para,
        type = "interactive",
        interactive = new
        {
            type = "list",
            body = new { text = texto },
            action = new
            {
                button = Recortar(textoBoton, 20),
                sections = new[] { new { title = "Opciones", rows = opciones.Take(MaximoFilas).Select(o => new { id = o.Id, title = Recortar(o.Titulo, 24), description = Recortar(o.Descripcion, 72) }).ToArray() } }
            }
        }
    };

    public static string Recortar(string texto, int maximo) => texto.Length <= maximo ? texto : string.Concat(texto.AsSpan(0, maximo - 1), "…");
}
