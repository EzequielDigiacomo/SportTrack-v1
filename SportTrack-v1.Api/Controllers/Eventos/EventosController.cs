using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Evento;
using SportTrack_v1.Controladores.Evento.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Eventos
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : ControllerBase
    {
        private readonly IEventoService _eventoService;

        public EventosController(IEventoService eventoService)
        {
            _eventoService = eventoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoDto>>> GetEventos()
        {
            var result = await _eventoService.GetAllEventosAsync();
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
        // [Authorize(Roles = "Admin")] // Habilitar cuando esté la auth lista
        public async Task<ActionResult<EventoDto>> CreateEvento(EventoCreateDto eventoDto)
        {
            var result = await _eventoService.CreateEventoAsync(eventoDto);
            return CreatedAtAction(nameof(GetEvento), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<EventoDto>> UpdateEvento(int id, EventoUpdateDto eventoDto)
        {
            var result = await _eventoService.UpdateEventoAsync(id, eventoDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            await _eventoService.DeleteEventoAsync(id);
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
