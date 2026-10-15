namespace Adif.Application.Services;

// Pasos y acciones de la máquina de estados del bot; también son los ids de los botones.
internal static class PasosBot
{
    public const string Inicio = "";
    public const string Nombre = "nombre";
    public const string Elegir = "elegir";
    public const string Monto = "monto";
    public const string OfrecerCrear = "ofrecer_crear";
    public const string NuevoNombre = "nuevo_nombre";
    public const string NuevoTelefono = "nuevo_tel";

    public const string AccionFiado = "fiado";
    public const string AccionAbono = "abono";
    public const string AccionSaldo = "saldo";

    public const string MenuFiado = "menu_fiado";
    public const string MenuAbono = "menu_abono";
    public const string MenuSaldo = "menu_saldo";
    public const string MenuDeudores = "menu_deudores";
    public const string MenuNuevo = "menu_nuevo";
    public const string Menu = "menu";
    public const string Cancelar = "cancelar";
    public const string CrearSi = "crear_si";
    public const string Omitir = "omitir";
    public const string PrefijoCliente = "cli_";
    public const string PrefijoFiar = "fiar_";
    public const string PrefijoAbonar = "abonar_";
}
