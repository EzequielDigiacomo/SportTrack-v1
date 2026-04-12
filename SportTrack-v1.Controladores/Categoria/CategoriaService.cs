using AutoMapper;
using SportTrack_v1.Controladores.Categoria.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Controladores.Categoria
{
    public class CategoriaService : ICategoriaService
    {
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IMapper _mapper;

    public CategoriaService(ICategoriaRepository categoriaRepository, IMapper mapper)
    {
        _categoriaRepository = categoriaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoriaDto>> GetAllCategoriasAsync()
    {
        var categorias = await _categoriaRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
    }

    public async Task<CategoriaDto> GetCategoriaByIdAsync(int id)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id);
        if (categoria == null)
            throw new NotFoundException($"Categoría con ID {id} no encontrada");

        return _mapper.Map<CategoriaDto>(categoria);
    }

    public async Task<CategoriaDto> CreateCategoriaAsync(CategoriaCreateDto categoriaDto)
    {
        // Validar que no haya superposición de edades si es necesario
        await ValidateCategoriaEdades(categoriaDto.EdadMin, categoriaDto.EdadMax);

        var categoria = _mapper.Map<Entidades.Entidades.Categoria>(categoriaDto);
        var createdCategoria = await _categoriaRepository.CreateAsync(categoria);
        return _mapper.Map<CategoriaDto>(createdCategoria);
    }

    public async Task<CategoriaDto> UpdateCategoriaAsync(int id, CategoriaUpdateDto categoriaDto)
    {
        var existingCategoria = await _categoriaRepository.GetByIdAsync(id);
        if (existingCategoria == null)
            throw new NotFoundException($"Categoría con ID {id} no encontrada");

        // Validar que no haya superposición de edades si es necesario
        await ValidateCategoriaEdades(categoriaDto.EdadMin, categoriaDto.EdadMax, id);

        _mapper.Map(categoriaDto, existingCategoria);
        var updatedCategoria = await _categoriaRepository.UpdateAsync(existingCategoria);
        return _mapper.Map<CategoriaDto>(updatedCategoria);
    }

    public async Task<bool> DeleteCategoriaAsync(int id)
    {
        if (!await _categoriaRepository.ExistsAsync(id))
            throw new NotFoundException($"Categoría con ID {id} no encontrada");

        return await _categoriaRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<CategoriaEdadDto>> GetCategoriasEdadAsync()
    {
        var categoriasEdad = Enum.GetValues<Entidades.Enums.CategoriaEdadEnum>()
            .Select(e => new CategoriaEdadDto
            {
                Id = (int)e,
                Nombre = e.GetDisplayName(),
                Valor = e.ToString()
            });

        return await Task.FromResult(categoriasEdad);
    }

    public async Task<IEnumerable<CategoriaDto>> GetCategoriasByEdadAsync(int edad)
    {
        var categorias = await _categoriaRepository.GetByEdadAsync(edad);
        return _mapper.Map<IEnumerable<CategoriaDto>>(categorias);
    }

    private async Task ValidateCategoriaEdades(int? edadMin, int? edadMax, int? excludeId = null)
    {
        // Implementar lógica de validación de superposición de rangos de edad
        // según las necesidades del negocio
    }
    }
}