// Recurso inexistente (404).
namespace Adif.Application.Common.Exceptions;

public sealed class NotFoundException(string message) : AppException(message);
