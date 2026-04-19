// Controllers/DistanciasController.cs
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Distancia;
using SportTrack_v1.Controladores.Distancia.Dtos;

[ApiController]
[Route("api/[controller]")]
public class DistanciasController : ControllerBase
{
    private readonly IDistanciaService _distanciaService;

    public DistanciasController(IDistanciaService distanciaService)
    {
        _distanciaService = distanciaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DistanciaDto>>> GetDistancias()
    {
        var distancias = await _distanciaService.GetAllDistanciasAsync();
        return Ok(distancias);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DistanciaDto>> GetDistancia(int id)
    {
        var distancia = await _distanciaService.GetDistanciaByIdAsync(id);
        return Ok(distancia);
    }

    [HttpPost]
    public async Task<ActionResult<DistanciaDto>> CreateDistancia(DistanciaCreateDto distanciaDto)
    {
        var createdDistancia = await _distanciaService.CreateDistanciaAsync(distanciaDto);
        return CreatedAtAction(nameof(GetDistancia), new { id = createdDistancia.Id }, createdDistancia);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<DistanciaDto>> UpdateDistancia(int id, DistanciaUpdateDto distanciaDto)
    {
        var updatedDistancia = await _distanciaService.UpdateDistanciaAsync(id, distanciaDto);
        return Ok(updatedDistancia);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteDistancia(int id)
    {
        await _distanciaService.DeleteDistanciaAsync(id);
        return NoContent();
    }

    [HttpGet("regata-tipos")]
    public async Task<ActionResult<IEnumerable<DistanciaRegataDto>>> GetDistanciasRegata()
    {
        var distancias = await _distanciaService.GetDistanciasRegataAsync();
        return Ok(distancias);
    }
}