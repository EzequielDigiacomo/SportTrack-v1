// Controllers/BotesController.cs
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Bote;
using SportTrack_v1.Controladores.Bote.Dtos;

[ApiController]
[Route("api/[controller]")]
public class BotesController : ControllerBase
{
    private readonly IBoteService _boteService;

    public BotesController(IBoteService boteService)
    {
        _boteService = boteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BoteDto>>> GetBotes()
    {
        var botes = await _boteService.GetAllBotesAsync();
        return Ok(botes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BoteDto>> GetBote(int id)
    {
        var bote = await _boteService.GetBoteByIdAsync(id);
        return Ok(bote);
    }

    [HttpPost]
    public async Task<ActionResult<BoteDto>> CreateBote(BoteCreateDto boteDto)
    {
        var createdBote = await _boteService.CreateBoteAsync(boteDto);
        return CreatedAtAction(nameof(GetBote), new { id = createdBote.Id }, createdBote);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BoteDto>> UpdateBote(int id, BoteUpdateDto boteDto)
    {
        var updatedBote = await _boteService.UpdateBoteAsync(id, boteDto);
        return Ok(updatedBote);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBote(int id)
    {
        await _boteService.DeleteBoteAsync(id);
        return NoContent();
    }

    [HttpGet("tipos")]
    public async Task<ActionResult<IEnumerable<TipoBoteDto>>> GetTiposBote()
    {
        var tipos = await _boteService.GetTiposBoteAsync();
        return Ok(tipos);
    }
}