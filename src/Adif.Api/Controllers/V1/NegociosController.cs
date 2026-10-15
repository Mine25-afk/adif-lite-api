using Adif.Application.Dtos;
using Adif.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers.V1;

[Route(ApiRoutes.V1 + "/negocios")]
public sealed class NegociosController(INegocioService negocios) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType<NegocioDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear(CrearNegocioDto dto, CancellationToken ct) => StatusCode(StatusCodes.Status201Created, await negocios.CrearAsync(Uid, dto, ct));

    [HttpGet("mios")]
    public async Task<ActionResult<IReadOnlyList<NegocioDto>>> ListarMios(CancellationToken ct) => Ok(await negocios.ListarMiosAsync(Uid, ct));

    [HttpPost("operadores")]
    public async Task<IActionResult> AgregarOperador(AgregarOperadorDto dto, CancellationToken ct)
    {
        var creado = await negocios.AgregarOperadorAsync(Uid, dto, ct);
        return creado ? StatusCode(StatusCodes.Status201Created, new { creado }) : Ok(new { creado });
    }
}
