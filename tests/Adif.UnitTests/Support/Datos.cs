using Adif.Domain.Entities;

namespace Adif.UnitTests.Support;

// Fábricas de entidades de prueba con valores razonables por defecto.
public static class Datos
{
    public const int NegocioId = 1;
    public const string Uid = "uid-dueno";

    public static Producto Producto(int id = 10, decimal precio = 20m, decimal stock = 50m, decimal mayoreo = 0m, int negocioId = NegocioId) => new()
    {
        Id = id,
        NegocioId = negocioId,
        Nombre = $"Producto {id}",
        Precio = precio,
        PrecioMayoreo = mayoreo,
        Costo = precio / 2,
        Stock = stock
    };

    public static Venta Credito(int id, decimal saldo, DateTime fecha, int socioId = 5) => new()
    {
        Id = id,
        NegocioId = NegocioId,
        SocioId = socioId,
        Fecha = fecha,
        EsCredito = true,
        Total = saldo,
        Saldo = saldo
    };

    public static Socio Cliente(int id = 5, string nombre = "María López") => new() { Id = id, NegocioId = NegocioId, Nombre = nombre };
}
