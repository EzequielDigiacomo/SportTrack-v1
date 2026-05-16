using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Participante;
using SportTrack_v1.Controladores.Participante.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Participantes
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantesController : ControllerBase
    {
        private readonly IParticipanteService _participanteService;

        public ParticipantesController(IParticipanteService participanteService)
        {
            _participanteService = participanteService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipanteDto>>> GetParticipantes()
        {
            var clubIdClaim = User.FindFirst("ClubId")?.Value;
            var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            int? clubId = string.IsNullOrEmpty(clubIdClaim) ? null : int.Parse(clubIdClaim);

            var result = await _participanteService.GetAllParticipantesAsync(clubId, roleClaim);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ParticipanteDto>> GetParticipante(int id)
        {
            var result = await _participanteService.GetParticipanteByIdAsync(id);
            return Ok(result);
        }

        [HttpGet("club/{clubId}")]
        public async Task<ActionResult<IEnumerable<ParticipanteDto>>> GetByClub(int clubId)
        {
            var result = await _participanteService.GetParticipantesByClubAsync(clubId);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ParticipanteDto>> CreateParticipante(ParticipanteCreateDto participanteDto)
        {
            var result = await _participanteService.CreateParticipanteAsync(participanteDto);
            return CreatedAtAction(nameof(GetParticipante), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ParticipanteDto>> UpdateParticipante(int id, ParticipanteCreateDto participanteDto)
        {
            var result = await _participanteService.UpdateParticipanteAsync(id, participanteDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteParticipante(int id)
        {
            await _participanteService.DeleteParticipanteAsync(id);
            return NoContent();
        }
    }
}
