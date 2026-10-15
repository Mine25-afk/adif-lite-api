namespace Adif.Application.Common;

// Acota página y tamaño para que un cliente no pida la tabla completa.
public static class Paginacion
{
    public const int TamanoPorDefecto = 20;
    public const int TamanoMaximo = 100;

    public static (int Pagina, int Tamano) Normalizar(int pagina, int tamano) => (Math.Max(1, pagina), tamano <= 0 ? TamanoPorDefecto : Math.Min(tamano, TamanoMaximo));
}
