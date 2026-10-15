// Renglón solicitado al registrar una venta.
namespace Adif.Domain.Entities;

public sealed record LineaVenta(Producto Producto, decimal Cantidad, bool PrecioMayoreo = false);
