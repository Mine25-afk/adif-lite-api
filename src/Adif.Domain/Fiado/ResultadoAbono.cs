// Resultado de repartir un abono entre los créditos abiertos de un cliente.
using Adif.Domain.Entities;

namespace Adif.Domain.Fiado;

public sealed record ResultadoAbono(decimal Aplicado, decimal Sobrante, IReadOnlyList<Abono> Abonos);
