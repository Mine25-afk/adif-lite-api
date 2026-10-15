// Alta de una empresa con su número de WhatsApp Business y su operador inicial.
namespace Adif.Application.Dtos;

public sealed class CrearNegocioDto
{
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Rtn { get; set; } = string.Empty;
    public string PhoneNumberId { get; set; } = string.Empty;
    public string DisplayNumber { get; set; } = string.Empty;
    public string WabaId { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string KapsoApiKey { get; set; } = string.Empty;
    public string OperadorTelefono { get; set; } = string.Empty;
    public string OperadorNombre { get; set; } = string.Empty;
}
