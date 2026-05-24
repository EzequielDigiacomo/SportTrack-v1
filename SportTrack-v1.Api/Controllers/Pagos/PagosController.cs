using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Pago;
using SportTrack_v1.Controladores.Pago.Dtos;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Pagos
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PagosController : ControllerBase
    {
        private readonly IPagoService _pagoService;

        public PagosController(IPagoService pagoService)
        {
            _pagoService = pagoService;
        }

        [HttpGet("historial")]
        public async Task<ActionResult<IEnumerable<PagoDto>>> GetHistorial()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var clubIdClaim = User.FindFirst("ClubId")?.Value;
            int? fedId = null;
            if (int.TryParse(clubIdClaim, out int parsedId))
            {
                fedId = parsedId;
            }

            var result = await _pagoService.GetHistorialPagosAsync(fedId, role);
            return Ok(result);
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<PagoDto>> RegistrarPago(RegistrarPagoDto dto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? User.FindFirst(ClaimTypes.Name)?.Value 
                           ?? "System";

            var result = await _pagoService.RegistrarPagoAsync(dto, username);
            return Ok(result);
        }

        [HttpPut("clubes/{id}/toggle")]
        public async Task<IActionResult> ToggleClubPago(int id, [FromBody] bool alDia)
        {
            var result = await _pagoService.ToggleClubPagoStatusAsync(id, alDia);
            if (!result) return BadRequest("No se pudo actualizar el estado de pago del club");
            return NoContent();
        }

        [HttpPut("clubes/{id}/solicitar-pago")]
        public async Task<IActionResult> SolicitarPago(int id, [FromBody] bool pendiente)
        {
            var result = await _pagoService.SetSolicitudPagoPendienteAsync(id, pendiente);
            if (!result) return BadRequest("No se pudo registrar la solicitud de pago");
            return NoContent();
        }

        [HttpPut("atletas/{id}/toggle")]
        public async Task<IActionResult> ToggleAtletaPago(int id, [FromBody] bool alDia)
        {
            var result = await _pagoService.ToggleAtletaPagoStatusAsync(id, alDia);
            if (!result) return BadRequest("No se pudo actualizar el estado de pago del atleta");
            return NoContent();
        }

        [HttpPut("inscripciones/{id}/toggle")]
        public async Task<IActionResult> ToggleInscripcionPago(int id, [FromBody] bool pagado)
        {
            var result = await _pagoService.ToggleInscripcionPagoStatusAsync(id, pagado);
            if (!result) return BadRequest("No se pudo actualizar el estado de pago de la inscripción");
            return NoContent();
        }
    }
}
