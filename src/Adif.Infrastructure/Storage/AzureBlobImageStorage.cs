using Adif.Application.Abstractions.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace Adif.Infrastructure.Storage;

// Contenedor privado; el cliente se crea al primer uso para que la API arranque sin Blob configurado.
public sealed class AzureBlobImageStorage(IOptions<BlobStorageOptions> opciones) : IImageStorage
{
    private readonly Lazy<BlobContainerClient> _contenedor = new(() => CrearContenedor(opciones.Value));

    public async Task<string> SubirAsync(string ruta, Stream contenido, string contentType, CancellationToken ct = default)
    {
        await _contenedor.Value.CreateIfNotExistsAsync(PublicAccessType.None, cancellationToken: ct);
        await _contenedor.Value.GetBlobClient(ruta).UploadAsync(contenido, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = contentType } }, ct);
        return ruta;
    }

    public string? GenerarUrlLectura(string ruta, TimeSpan vigencia)
    {
        var blob = _contenedor.Value.GetBlobClient(ruta);
        return blob.CanGenerateSasUri ? blob.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.Add(vigencia)).ToString() : null;
    }

    public async Task EliminarAsync(string ruta, CancellationToken ct = default) => await _contenedor.Value.GetBlobClient(ruta).DeleteIfExistsAsync(cancellationToken: ct);

    private static BlobContainerClient CrearContenedor(BlobStorageOptions o)
    {
        if (string.IsNullOrWhiteSpace(o.ConnectionString)) throw new InvalidOperationException("Azure Blob Storage no está configurado (ConnectionStrings:Blob).");
        return new BlobServiceClient(o.ConnectionString).GetBlobContainerClient(o.Container);
    }
}
