// Estado en conflicto con la petición: duplicados o ambigüedad (409).
namespace Adif.Application.Common.Exceptions;

public sealed class ConflictException(string message) : AppException(message);
