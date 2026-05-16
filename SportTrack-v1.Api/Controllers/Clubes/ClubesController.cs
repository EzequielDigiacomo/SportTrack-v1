using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Club;
using SportTrack_v1.Controladores.Club.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Clubes
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClubesController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubesController(IClubService clubService)
        {
            _clubService = clubService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClubDto>>> GetClubes()
        {
            var result = await _clubService.GetAllClubesAsync();

            // Filtrar según el rol del usuario logueado
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var clubIdClaim = User.FindFirst("ClubId")?.Value;

            // Si es SuperAdmin ve todo
            if (role == "SuperAdmin")
                return Ok(result);

            // Si es Admin, solo ve los clubes de su federación (ParentClubId == su ClubId)
            // También incluimos su propio club raíz
            if (role == "Admin" && int.TryParse(clubIdClaim, out int fedId))
            {
                var filtered = result.Where(c => c.ParentClubId == fedId || c.Id == fedId);
                return Ok(filtered);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClubDto>> GetClub(int id)
        {
            var result = await _clubService.GetClubByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ClubDto>> CreateClub(ClubCreateDto clubDto)
        {
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (role == "Admin")
            {
                var clubIdClaim = User.FindFirst("ClubId")?.Value;
                if (int.TryParse(clubIdClaim, out int fedId) && fedId > 0)
                {
                    clubDto.ParentClubId = fedId;
                }
            }
            var result = await _clubService.CreateClubAsync(clubDto);
            return CreatedAtAction(nameof(GetClub), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ClubDto>> UpdateClub(int id, ClubUpdateDto clubDto)
        {
            var result = await _clubService.UpdateClubAsync(id, clubDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteClub(int id)
        {
            await _clubService.DeleteClubAsync(id);
            return NoContent();
        }
    }
}
