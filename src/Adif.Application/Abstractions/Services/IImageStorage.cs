namespace Adif.Application.Abstractions.Services;

// Almacén privado de imágenes; la lectura siempre es por URL firmada temporal.
public interface IImageStorage
{
    Task<string> SubirAsync(string ruta, Stream contenido, string contentType, CancellationToken ct = default);
    string? GenerarUrlLectura(string ruta, TimeSpan vigencia);
    Task EliminarAsync(string ruta, CancellationToken ct = default);
}
