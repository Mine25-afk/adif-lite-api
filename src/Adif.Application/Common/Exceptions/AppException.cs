// Base de los errores de aplicación que se traducen a un código HTTP.
namespace Adif.Application.Common.Exceptions;

public abstract class AppException(string message) : Exception(message);
