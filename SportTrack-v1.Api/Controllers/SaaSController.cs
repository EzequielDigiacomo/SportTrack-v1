using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.SaaS;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SaaSController : ControllerBase
    {
        private readonly ISaaSService _saasService;

        public SaaSController(ISaaSService saasService)
        {
            _saasService = saasService;
        }

        [HttpGet("planes")]
        public async Task<IActionResult> GetPlanes()
        {
            var planes = await _saasService.GetPlanesAsync();
            return Ok(planes);
        }

        [HttpPost("asignar-plan")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AsignarPlan(int clubId, int planId)
        {
            await _saasService.AsignarPlanAClubAsync(clubId, planId);
            return Ok(new { message = "Plan asignado correctamente" });
        }

        [HttpGet("clubes-status")]
        [Authorize(Roles = "SuperAdmin,Admin,soporte_tecnico")]
        public async Task<IActionResult> GetClubesStatus()
        {
            var clubesStatus = await _saasService.GetClubesStatusAsync();
            return Ok(clubesStatus);
        }

        [HttpPatch("clubes/{id}/toggle-activo")]
        [Authorize(Roles = "SuperAdmin,Admin,soporte_tecnico")]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            await _saasService.ToggleClubActivoAsync(id);
            return Ok(new { message = "Estado de federación actualizado correctamente" });
        }

        [HttpPost("create-federacion")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateFederacion([FromBody] SportTrack_v1.Controladores.SaaS.Dtos.SaaSCreateFederacionDto dto)
        {
            var id = await _saasService.CreateFederacionWithAdminAsync(dto);
            return Ok(new { id, message = "Federación y administrador creados correctamente" });
        }

        [HttpGet("global-metrics")]
        [Authorize(Roles = "SuperAdmin,soporte_tecnico")]
        public async Task<IActionResult> GetGlobalMetrics()
        {
            var metrics = await _saasService.GetGlobalMetricsAsync();
            return Ok(metrics);
        }
    }
}
