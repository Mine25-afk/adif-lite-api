using Adif.Application.Common;
using Adif.Application.Dtos;
using Adif.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers.V1;

[Route(ApiRoutes.V1 + "/ventas")]
public sealed class VentasController(IVentaService ventas) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType<VentaDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Registrar(RegistrarVentaDto dto, CancellationToken ct) => StatusCode(StatusCodes.Status201Created, await ventas.RegistrarAsync(Uid, dto, ct));

    [HttpGet]
    public async Task<ActionResult<PagedResult<VentaDto>>> Listar(int negocioId, DateTime? desde, DateTime? hasta, int pagina = 1, int tamano = Paginacion.TamanoPorDefecto, CancellationToken ct = default) => Ok(await ventas.ListarAsync(Uid, negocioId, desde, hasta, pagina, tamano, ct));

    [HttpGet("detalle")]
    public async Task<ActionResult<VentaDto>> Obtener(int negocioId, int id, CancellationToken ct) => Ok(await ventas.ObtenerAsync(Uid, negocioId, id, ct));

    [HttpPost("anular")]
    public async Task<ActionResult<VentaDto>> Anular(int negocioId, int id, CancellationToken ct) => Ok(await ventas.AnularAsync(Uid, negocioId, id, ct));
}
