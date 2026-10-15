using Adif.Application.Dtos;
using Adif.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Adif.Api.Controllers.V1;

[Route(ApiRoutes.V1 + "/licencias")]
public sealed class LicenciasController(ILicenciaService licencias) : ApiControllerBase
{
    [HttpPost("accesos")]
    public async Task<ActionResult<DispositivoDto>> RegistrarAcceso(RegistroAccesoDto dto, CancellationToken ct) => Ok(await licencias.RegistrarAccesoAsync(Uid, dto, ct));

    [HttpGet("dispositivos")]
    public async Task<ActionResult<IReadOnlyList<DispositivoDto>>> Dispositivos(CancellationToken ct) => Ok(await licencias.ListarDispositivosAsync(Uid, ct));

    [HttpPost("dispositivos")]
    [ProducesResponseType<DispositivoDto>(StatusCodes.Status201Created)]
    public async Task<IActionResult> CrearCuenta(CrearCuentaDto dto, CancellationToken ct) => StatusCode(StatusCodes.Status201Created, await licencias.CrearCuentaAsync(Uid, dto, ct));

    [HttpPut("dispositivos/bloqueo")]
    public async Task<ActionResult<DispositivoDto>> CambiarBloqueo(BloqueoDispositivoDto dto, CancellationToken ct) => Ok(await licencias.CambiarBloqueoAsync(Uid, dto, ct));

    [HttpPost("mensualidades")]
    public async Task<ActionResult<DispositivoDto>> ComprarMensualidad(CompraMensualidadDto dto, CancellationToken ct) => Ok(await licencias.ComprarMensualidadAsync(Uid, dto, ct));

    [HttpPost("codigos/canjear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> CanjearCodigo(ValidarCodigoDto dto, CancellationToken ct)
    {
        await licencias.CanjearCodigoAsync(dto, ct);
        return NoContent();
    }
}
