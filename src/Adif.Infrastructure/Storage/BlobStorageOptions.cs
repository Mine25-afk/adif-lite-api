// Configuración de Azure Blob Storage para imágenes de productos.
namespace Adif.Infrastructure.Storage;

public sealed class BlobStorageOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string Container { get; set; } = "productos";
}
