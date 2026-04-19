// Controllers/CategoriasController.cs
using Microsoft.AspNetCore.Mvc;
using SportTrack_v1.Controladores.Categoria;
using SportTrack_v1.Controladores.Categoria.Dtos;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategorias()
    {
        var categorias = await _categoriaService.GetAllCategoriasAsync();
        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoriaDto>> GetCategoria(int id)
    {
        var categoria = await _categoriaService.GetCategoriaByIdAsync(id);
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDto>> CreateCategoria(CategoriaCreateDto categoriaDto)
    {
        var createdCategoria = await _categoriaService.CreateCategoriaAsync(categoriaDto);
        return CreatedAtAction(nameof(GetCategoria), new { id = createdCategoria.Id }, createdCategoria);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CategoriaDto>> UpdateCategoria(int id, CategoriaUpdateDto categoriaDto)
    {
        var updatedCategoria = await _categoriaService.UpdateCategoriaAsync(id, categoriaDto);
        return Ok(updatedCategoria);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategoria(int id)
    {
        await _categoriaService.DeleteCategoriaAsync(id);
        return NoContent();
    }

    [HttpGet("tipos-edad")]
    public async Task<ActionResult<IEnumerable<CategoriaEdadDto>>> GetCategoriasEdad()
    {
        var categoriasEdad = await _categoriaService.GetCategoriasEdadAsync();
        return Ok(categoriasEdad);
    }

    [HttpGet("por-edad/{edad}")]
    public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetCategoriasByEdad(int edad)
    {
        var categorias = await _categoriaService.GetCategoriasByEdadAsync(edad);
        return Ok(categorias);
    }
}