// Violación de una regla de negocio del dominio.
namespace Adif.Domain.Common;

public class DomainException(string message) : Exception(message);
