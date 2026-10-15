using System.Security.Cryptography;
using System.Text;

namespace Adif.Application.Services;

// HMAC-SHA256 en hex sobre los bytes exactos del cuerpo, comparado en tiempo constante.
public static class FirmaWebhook
{
    public static string Calcular(string cuerpo, string secreto) => Convert.ToHexStringLower(HMACSHA256.HashData(Encoding.UTF8.GetBytes(secreto), Encoding.UTF8.GetBytes(cuerpo)));

    public static bool EsValida(string cuerpo, string? secreto, string? firma)
    {
        if (string.IsNullOrWhiteSpace(secreto) || string.IsNullOrWhiteSpace(firma)) return false;

        var esperada = Encoding.ASCII.GetBytes(Calcular(cuerpo, secreto));
        var recibida = Encoding.ASCII.GetBytes(firma.Trim().ToLowerInvariant());
        return CryptographicOperations.FixedTimeEquals(esperada, recibida);
    }
}
