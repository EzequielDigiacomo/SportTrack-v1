using AutoMapper;
using SportTrack_v1.Controladores.Club.Dtos;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Club
{
    public class ClubService : IClubService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IMapper _mapper;

        public ClubService(IClubRepository clubRepository, IMapper mapper)
        {
            _clubRepository = clubRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClubDto>> GetAllClubesAsync()
        {
            var clubes = await _clubRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClubDto>>(clubes);
        }

        public async Task<ClubDto> GetClubByIdAsync(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null) throw new NotFoundException($"Club con ID {id} no encontrado");
            return _mapper.Map<ClubDto>(club);
        }

        public async Task<ClubDto> CreateClubAsync(ClubCreateDto clubDto)
        {
            var club = _mapper.Map<Entidades.Entidades.Club>(clubDto);
            
            // Asignar plan por defecto (Bronce = 1) si no tiene uno
            if (club.PlanSaaSId == null || club.PlanSaaSId == 0)
            {
                club.PlanSaaSId = 1; 
            }

            var result = await _clubRepository.CreateAsync(club);
            return _mapper.Map<ClubDto>(result);
        }

        public async Task<ClubDto> UpdateClubAsync(int id, ClubUpdateDto clubDto)
        {
            var existing = await _clubRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Club con ID {id} no encontrado");
            
            _mapper.Map(clubDto, existing);
            var result = await _clubRepository.UpdateAsync(existing);
            return _mapper.Map<ClubDto>(result);
        }

        public async Task<bool> DeleteClubAsync(int id)
        {
            if (!await _clubRepository.ExistsAsync(id)) throw new NotFoundException($"Club con ID {id} no encontrado");
            return await _clubRepository.DeleteAsync(id);
        }
    }
}
