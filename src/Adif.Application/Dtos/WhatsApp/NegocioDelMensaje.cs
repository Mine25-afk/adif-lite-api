// Tenant y cuenta de envío resueltos para un mensaje entrante.
namespace Adif.Application.Dtos;

public sealed record NegocioDelMensaje(int NegocioId, CuentaWhatsapp Cuenta);
