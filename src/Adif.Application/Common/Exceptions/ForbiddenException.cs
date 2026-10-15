// Usuario autenticado sin permiso sobre el recurso (403).
namespace Adif.Application.Common.Exceptions;

public sealed class ForbiddenException(string message) : AppException(message);
