// Métodos de pago aceptados.
namespace Adif.Domain.Common;

public enum MetodoPago
{
    Efectivo = 0,
    Transferencia = 1,
    Tarjeta = 2,
    Otro = 9
}
