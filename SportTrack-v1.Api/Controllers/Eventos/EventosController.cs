using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Evento;
using SportTrack_v1.Controladores.Evento.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Eventos
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;
        private readonly SportTrack_v1.Controladores.Fase.IFaseService _faseService;

        public EventosController(IEventoService eventoService, SportTrack_v1.Controladores.Fase.IFaseService faseService)
        {
            _eventoService = eventoService;
            _faseService = faseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventos()
        {
            int? clubId = null;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            
            if (role == "Club")
            {
                var clubIdClaim = User.FindFirst("ClubId")?.Value;
                if (int.TryParse(clubIdClaim, out int id) && id > 0) clubId = id;
            }

            var result = await _eventoService.GetAllEventosAsync(clubId);
            return Ok(result);
        }

        [HttpGet("{id}/fases")]
        public async Task<ActionResult<IEnumerable<SportTrack_v1.Controladores.Fase.Dtos.FaseDto>>> GetFases(int id)
        {
            var result = await _faseService.GetFasesPorEventoAsync(id);
            return Ok(result);
        }

        [HttpGet("proximos")]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetProximosEventos()
        {
            var result = await _eventoService.GetProximosEventosAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
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
