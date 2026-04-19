using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Inscripcion;
using SportTrack_v1.Controladores.Inscripcion.Dtos;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Inscripciones
{
    [ApiController]
    [Route("api/[controller]")]
    public class InscripcionesController : ControllerBase
    {
        private readonly IInscripcionService _inscripcionService;

        public InscripcionesController(IInscripcionService inscripcionService)
        {
            _inscripcionService = inscripcionService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetInscripciones()
        {
            var result = await _inscripcionService.GetAllInscripcionesAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InscripcionDto>> GetInscripcion(int id)
        {
            var result = await _inscripcionService.GetInscripcionByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize] // Solo usuarios registrados (Clubes o Admin) pueden inscribir
        public async Task<ActionResult<InscripcionDto>> CreateInscripcion(InscripcionCreateDto inscripcionDto)
        {
            // Opcional: Validar que el clubId del usuario coincida con el club del participante
            // var clubId = User.FindFirst("ClubId")?.Value;
            
            var result = await _inscripcionService.CreateInscripcionAsync(inscripcionDto);
            return CreatedAtAction(nameof(GetInscripcion), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<InscripcionDto>> UpdateInscripcion(int id, InscripcionUpdateDto inscripcionDto)
        {
            var result = await _inscripcionService.UpdateInscripcionAsync(id, inscripcionDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteInscripcion(int id)
        {
            await _inscripcionService.DeleteInscripcionAsync(id);
            return NoContent();
        }

        [HttpGet("evento-prueba/{eventoPruebaId}")]
        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetByEventoPrueba(int eventoPruebaId)
        {
            var result = await _inscripcionService.GetInscripcionesByEventoPruebaIdAsync(eventoPruebaId);
            return Ok(result);
        }

        [HttpGet("evento/{eventoId}/club/{clubId}")]
        public async Task<ActionResult<IEnumerable<InscripcionDto>>> GetByEventoAndClub(int eventoId, int clubId)
        {
            var result = await _inscripcionService.GetInscripcionesByEventoAndClubAsync(eventoId, clubId);
            return Ok(result);
        }

        [HttpPatch("{id}/toggle-seeding")]
        [Authorize]
        public async Task<ActionResult> ToggleSeeding(int id)
        {
            await _inscripcionService.ToggleEsCabezaDeSerieAsync(id);
            return Ok();
        }
    }
}
