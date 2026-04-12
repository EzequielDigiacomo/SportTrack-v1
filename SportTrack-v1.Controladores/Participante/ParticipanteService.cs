using AutoMapper;
using SportTrack_v1.Controladores.Exceptions;
using SportTrack_v1.Controladores.Participante.Dtos;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Participante
{
    public class ParticipanteService : IParticipanteService
    {
        private readonly IParticipanteRepository _participanteRepository;
        private readonly IMapper _mapper;

        public ParticipanteService(IParticipanteRepository participanteRepository, IMapper mapper)
        {
            _participanteRepository = participanteRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ParticipanteDto>> GetAllParticipantesAsync()
        {
            var participantes = await _participanteRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ParticipanteDto>>(participantes);
        }

        public async Task<ParticipanteDto> GetParticipanteByIdAsync(int id)
        {
            var participante = await _participanteRepository.GetByIdAsync(id);
            if (participante == null) throw new NotFoundException($"Participante con ID {id} no encontrado");
            return _mapper.Map<ParticipanteDto>(participante);
        }

        public async Task<IEnumerable<ParticipanteDto>> GetParticipantesByClubAsync(int clubId)
        {
            var participantes = await _participanteRepository.GetByClubIdAsync(clubId);
            return _mapper.Map<IEnumerable<ParticipanteDto>>(participantes);
        }

        public async Task<ParticipanteDto> CreateParticipanteAsync(ParticipanteCreateDto participanteDto)
        {
            var participante = _mapper.Map<Entidades.Entidades.Participante>(participanteDto);
            var result = await _participanteRepository.CreateAsync(participante);
            // Recargar para traer navegaciones
            var fullResult = await _participanteRepository.GetByIdAsync(result.Id);
            return _mapper.Map<ParticipanteDto>(fullResult);
        }

        public async Task<ParticipanteDto> UpdateParticipanteAsync(int id, ParticipanteCreateDto participanteDto)
        {
            var existing = await _participanteRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Participante con ID {id} no encontrado");
            
            _mapper.Map(participanteDto, existing);
            var result = await _participanteRepository.UpdateAsync(existing);
            return _mapper.Map<ParticipanteDto>(result);
        }

        public async Task<bool> DeleteParticipanteAsync(int id)
        {
            if (!await _participanteRepository.ExistsAsync(id)) throw new NotFoundException($"Participante con ID {id} no encontrado");
            return await _participanteRepository.DeleteAsync(id);
        }
    }
}
