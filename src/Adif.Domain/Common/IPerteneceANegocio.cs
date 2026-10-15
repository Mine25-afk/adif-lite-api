// Contrato de las entidades que viven dentro de un negocio (tenant).
namespace Adif.Domain.Common;

public interface IPerteneceANegocio
{
    int NegocioId { get; }
}
