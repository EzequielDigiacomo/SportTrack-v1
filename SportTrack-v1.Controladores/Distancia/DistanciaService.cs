using AutoMapper;
using SportTrack_v1.Controladores.Distancia.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Controladores.Distancia
{
    public class DistanciaService : IDistanciaService
    {
    private readonly IDistanciaRepository _distanciaRepository;
    private readonly IMapper _mapper;

    public DistanciaService(IDistanciaRepository distanciaRepository, IMapper mapper)
    {
        _distanciaRepository = distanciaRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<DistanciaDto>> GetAllDistanciasAsync()
    {
        var distancias = await _distanciaRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DistanciaDto>>(distancias);
    }

    public async Task<DistanciaDto> GetDistanciaByIdAsync(int id)
    {
        var distancia = await _distanciaRepository.GetByIdAsync(id);
        if (distancia == null)
            throw new NotFoundException($"Distancia con ID {id} no encontrada");

        return _mapper.Map<DistanciaDto>(distancia);
    }

    public async Task<DistanciaDto> CreateDistanciaAsync(DistanciaCreateDto distanciaDto)
    {
        var distancia = _mapper.Map<Entidades.Entidades.Distancia>(distanciaDto);
        var createdDistancia = await _distanciaRepository.CreateAsync(distancia);
        return _mapper.Map<DistanciaDto>(createdDistancia);
    }

    public async Task<DistanciaDto> UpdateDistanciaAsync(int id, DistanciaUpdateDto distanciaDto)
    {
        var existingDistancia = await _distanciaRepository.GetByIdAsync(id);
        if (existingDistancia == null)
            throw new NotFoundException($"Distancia con ID {id} no encontrada");

        _mapper.Map(distanciaDto, existingDistancia);
        var updatedDistancia = await _distanciaRepository.UpdateAsync(existingDistancia);
        return _mapper.Map<DistanciaDto>(updatedDistancia);
    }

    public async Task<bool> DeleteDistanciaAsync(int id)
    {
        if (!await _distanciaRepository.ExistsAsync(id))
            throw new NotFoundException($"Distancia con ID {id} no encontrada");

        return await _distanciaRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<DistanciaRegataDto>> GetDistanciasRegataAsync()
    {
        var distancias = Enum.GetValues<Entidades.Enums.DistanciaRegataEnum>()
            .Select(e => new DistanciaRegataDto
            {
                Id = (int)e,
                Metros = (int)e,
                Descripcion = e.GetDisplayName(),
                Valor = e.ToString()
            });

        return await Task.FromResult(distancias);
    }
    }
}