// Configuración de Kapso (proxy del Cloud API de WhatsApp).
namespace Adif.Infrastructure.WhatsApp;

public sealed class KapsoOptions
{
    public string BaseUrl { get; set; } = "https://api.kapso.ai/meta/whatsapp/v24.0";
    public string ProjectApiKey { get; set; } = string.Empty;
    public string ToolsApiKey { get; set; } = string.Empty;
    public int TimeoutSegundos { get; set; } = 15;
}
