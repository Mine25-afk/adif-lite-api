// Página de resultados con su conteo total.
namespace Adif.Application.Common;

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public int Pagina { get; init; } = 1;
    public int TamanoPagina { get; init; } = Paginacion.TamanoPorDefecto;
    public int Total { get; init; }
    public int TotalPaginas => TamanoPagina <= 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanoPagina);

    public PagedResult<TDestino> Map<TDestino>(Func<T, TDestino> mapeo) => new() { Items = Items.Select(mapeo).ToList(), Pagina = Pagina, TamanoPagina = TamanoPagina, Total = Total };
}
