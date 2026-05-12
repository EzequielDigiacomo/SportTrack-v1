using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Fase;
using SportTrack_v1.Controladores.Fase.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FasesController : ControllerBase
    {
        private readonly IFaseService _faseService;

        public FasesController(IFaseService faseService)
        {
            _faseService = faseService;
        }

        [HttpGet("EventoPrueba/{eventoPruebaId}")]
        public async Task<ActionResult<IEnumerable<FaseDto>>> GetFasesPorEventoPrueba(int eventoPruebaId)
        {
            var fases = await _faseService.GetFasesPorEventoPruebaAsync(eventoPruebaId);
            return Ok(fases);
        }

        [HttpGet("all-by-evento/{eventoId}")]
        public async Task<ActionResult<IEnumerable<FaseDto>>> GetFasesPorEvento(int eventoId)
        {
            var fases = await _faseService.GetFasesPorEventoAsync(eventoId);
            return Ok(fases);
        }

        [HttpPost("BatchUpdate")]
        public async Task<ActionResult> BatchUpdate([FromBody] List<FaseBatchUpdateDto> dto)
        {
            await _faseService.BatchUpdateFasesAsync(dto);
            return Ok();
        }

        [HttpPost("Generar/{eventoPruebaId}")]
        public async Task<ActionResult<IEnumerable<FaseDto>>> GenerarFases(int eventoPruebaId)
        {
            var fases = await _faseService.GenerarFasesAutoAsync(eventoPruebaId);
            return Ok(fases);
        }

        [HttpPost("GenerarManual/{eventoPruebaId}")]
        public async Task<ActionResult<IEnumerable<FaseDto>>> GenerarFasesManual(int eventoPruebaId, [FromBody] List<ManualPlacementDto> placements)
        {
            var fases = await _faseService.GenerarFasesManualAsync(eventoPruebaId, placements);
            return Ok(fases);
        }

        [HttpPost("Promover/{eventoPruebaId}")]
        public async Task<ActionResult<IEnumerable<FaseDto>>> Promover(int eventoPruebaId)
        {
            var fases = await _faseService.PromoverFasesAsync(eventoPruebaId);
            return Ok(fases);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _faseService.DeleteFaseAsync(id);
            return NoContent();
        }

        [HttpPost("{id}/Iniciar")]
        public async Task<ActionResult<FaseDto>> Iniciar(int id)
        {
            var fase = await _faseService.IniciarFaseAsync(id);
            return Ok(fase);
        }

        [HttpPost("{id}/Finalizar")]
        public async Task<ActionResult<FaseDto>> Finalizar(int id)
        {
            var fase = await _faseService.FinalizarFaseAsync(id);
            return Ok(fase);
        }

        [HttpPost("{id}/Reiniciar")]
        public async Task<ActionResult<FaseDto>> Reiniciar(int id)
        {
            var fase = await _faseService.ReiniciarFaseAsync(id);
            return Ok(fase);
        }
        
        [HttpPost("{id}/EnviarARevision")]
        public async Task<ActionResult<FaseDto>> EnviarARevision(int id)
        {
            var fase = await _faseService.EnviarARevisionAsync(id);
            return Ok(fase);
        }
            }
}
