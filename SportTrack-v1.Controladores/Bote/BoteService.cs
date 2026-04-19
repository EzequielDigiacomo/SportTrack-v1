using AutoMapper;
using SportTrack_v1.Controladores.Bote.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Controladores.Bote
{
    public class BoteService : IBoteService
    {
    private readonly IBoteRepository _boteRepository;
    private readonly IMapper _mapper;

    public BoteService(IBoteRepository boteRepository, IMapper mapper)
    {
        _boteRepository = boteRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BoteDto>> GetAllBotesAsync()
    {
        var botes = await _boteRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<BoteDto>>(botes);
    }

    public async Task<BoteDto> GetBoteByIdAsync(int id)
    {
        var bote = await _boteRepository.GetByIdAsync(id);
        if (bote == null)
            throw new NotFoundException($"Bote con ID {id} no encontrado");

        return _mapper.Map<BoteDto>(bote);
    }

    public async Task<BoteDto> CreateBoteAsync(BoteCreateDto boteDto)
    {
        var bote = _mapper.Map<Entidades.Entidades.Bote>(boteDto);
        var createdBote = await _boteRepository.CreateAsync(bote);
        return _mapper.Map<BoteDto>(createdBote);
    }

    public async Task<BoteDto> UpdateBoteAsync(int id, BoteUpdateDto boteDto)
    {
        var existingBote = await _boteRepository.GetByIdAsync(id);
        if (existingBote == null)
            throw new NotFoundException($"Bote con ID {id} no encontrado");

        _mapper.Map(boteDto, existingBote);
        var updatedBote = await _boteRepository.UpdateAsync(existingBote);
        return _mapper.Map<BoteDto>(updatedBote);
    }

    public async Task<bool> DeleteBoteAsync(int id)
    {
        if (!await _boteRepository.ExistsAsync(id))
            throw new NotFoundException($"Bote con ID {id} no encontrado");

        return await _boteRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TipoBoteDto>> GetTiposBoteAsync()
    {
        var tipos = Enum.GetValues<Entidades.Enums.TipoBoteEnum>()
            .Select(e => new TipoBoteDto
            {
                Id = (int)e,
                Nombre = e.GetDisplayName(),
                Valor = e.ToString()
            });

        return await Task.FromResult(tipos);
    }
    }
}
