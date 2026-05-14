using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Evento;
using SportTrack_v1.Controladores.Evento.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Fase;
using SportTrack_v1.Controladores.Fase.Dtos;

namespace SportTrack_v1.Api.Controllers.Eventos
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly IFaseService _faseService;

        public EventosController(IEventoService eventoService, IFaseService faseService)
        {
            _eventoService = eventoService;
            _faseService = faseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventos([FromQuery] int? clubId = null)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            
            // Si no es SuperAdmin o no se pasó un clubId, usamos el del Token
            if (role != "SuperAdmin" || !clubId.HasValue)
            {
                var clubIdClaim = User.FindFirst("ClubId")?.Value;
                if (int.TryParse(clubIdClaim, out int id)) clubId = id;
            }

            var result = await _eventoService.GetAllEventosAsync(clubId, role);
            return Ok(result);
        }

        [HttpGet("{id}/fases")]
        public async Task<ActionResult<IEnumerable<FaseDto>>> GetFases(int id)
        {
            var result = await _faseService.GetFasesPorEventoAsync(id);
            return Ok(result);
        }

        [HttpGet("proximos")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetProximosEventos([FromQuery] int? clubId = null)
        {
            string? role = null;

            // Si el usuario está logueado
            if (User.Identity?.IsAuthenticated == true)
            {
                role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // Si no es SuperAdmin o no hay override, usamos el del Token
                if (role != "SuperAdmin" || !clubId.HasValue)
                {
                    var clubIdClaim = User.FindFirst("ClubId")?.Value;
                    if (int.TryParse(clubIdClaim, out int id)) clubId = id;
                }
            }

            var result = await _eventoService.GetProximosEventosAsync(clubId, role);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventoDto>> GetEvento(int id)
        {
            var result = await _eventoService.GetEventoByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EventoDto>> CreateEvento(EventoCreateDto eventoDto)
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Club")
            {
                var clubIdClaim = User.FindFirst("ClubId")?.Value;
                if (int.TryParse(clubIdClaim, out int id) && id > 0) eventoDto.ClubId = id;
            }

            var result = await _eventoService.CreateEventoAsync(eventoDto);
            return CreatedAtAction(nameof(GetEvento), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EventoDto>> UpdateEvento(int id, EventoUpdateDto eventoDto)
        {
            int? clubId = null;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Club")
            {
                var clubIdClaim = User.FindFirst("ClubId")?.Value;
                if (int.TryParse(clubIdClaim, out int cid) && cid > 0) clubId = cid;
            }

            var result = await _eventoService.UpdateEventoAsync(id, eventoDto, clubId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            int? clubId = null;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == "Club")
            {
                var clubIdClaim = User.FindFirst("ClubId")?.Value;
                if (int.TryParse(clubIdClaim, out int cid) && cid > 0) clubId = cid;
            }

            await _eventoService.DeleteEventoAsync(id, clubId);
            return NoContent();
        }

        [HttpGet("{id}/pruebas")]
        public async Task<ActionResult<IEnumerable<EventoPruebaDto>>> GetPruebas(int id)
        {
            var result = await _eventoService.GetPruebasByEventoAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/pruebas")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EventoPruebaDto>> AssignPrueba(int id, EventoPruebaCreateDto assignDto)
        {
            var result = await _eventoService.AssignPruebaToEventoAsync(id, assignDto);
            return Ok(result);
        }

        [HttpPut("pruebas/{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EventoPruebaDto>> UpdatePrueba(int id, EventoPruebaCreateDto updateDto)
        {
            var result = await _eventoService.UpdateEventoPruebaAsync(id, updateDto);
            return Ok(result);
        }

        [HttpDelete("pruebas/{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnassignPrueba(int id)
        {
            await _eventoService.DeleteEventoPruebaAsync(id);
            return NoContent();
        }
    }
}
