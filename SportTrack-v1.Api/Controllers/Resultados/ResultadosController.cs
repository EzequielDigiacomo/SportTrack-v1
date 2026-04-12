using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Resultado;
using SportTrack_v1.Controladores.Resultado.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers.Resultados
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResultadosController : ControllerBase
    {
        private readonly IResultadoService _resultadoService;

        public ResultadosController(IResultadoService resultadoService)
        {
            _resultadoService = resultadoService;
        }

        [HttpGet("prueba/{eventoPruebaId}")]
        public async Task<ActionResult<IEnumerable<ResultadoDto>>> GetResultadosByPrueba(int eventoPruebaId)
        {
            // Este endpoint es público para que cualquier usuario vea los resultados en TR
            var result = await _resultadoService.GetResultadosByPruebaAsync(eventoPruebaId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ResultadoDto>> GetResultado(int id)
        {
            var result = await _resultadoService.GetResultadoByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("upsert")]
        // [Authorize(Roles = "Admin,Judge")]
        public async Task<ActionResult<ResultadoDto>> UpsertResultado(ResultadoCreateDto resultadoDto)
        {
            // Este endpoint carga o actualiza un tiempo y notifica vía SignalR
            var result = await _resultadoService.UpsertResultadoAsync(resultadoDto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteResultado(int id)
        {
            await _resultadoService.DeleteResultadoAsync(id);
            return NoContent();
        }
    }
}
