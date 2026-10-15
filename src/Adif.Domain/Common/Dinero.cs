// Redondeo monetario único del dominio.
namespace Adif.Domain.Common;

public static class Dinero
{
    public static decimal Redondear(decimal valor) => Math.Round(valor, 2, MidpointRounding.AwayFromZero);
}
