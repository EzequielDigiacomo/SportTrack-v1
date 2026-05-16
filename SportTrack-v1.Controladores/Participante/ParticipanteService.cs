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
        private readonly Club.IClubRepository _clubRepository;
        private readonly IMapper _mapper;
        private readonly Audit.IAuditService _auditService;

        public ParticipanteService(IParticipanteRepository participanteRepository, Club.IClubRepository clubRepository, IMapper mapper, Audit.IAuditService auditService)
        {
            _participanteRepository = participanteRepository;
            _clubRepository = clubRepository;
            _mapper = mapper;
            _auditService = auditService;
        }

        public async Task<IEnumerable<ParticipanteDto>> GetAllParticipantesAsync(int? clubId = null, string? rol = null)
        {
            IEnumerable<Entidades.Entidades.Participante> participantes;

            if (rol == "SuperAdmin")
            {
                participantes = await _participanteRepository.GetAllAsync();
            }
            else if (rol == "Admin" && clubId.HasValue)
            {
                // En este sistema, 'Admin' es el rol de la Federación (como FACA)
                participantes = await _participanteRepository.GetByFederationIdAsync(clubId.Value);
            }
            else if (clubId.HasValue)
            {
                // 'Club' y otros roles ven solo lo de su club
                participantes = await _participanteRepository.GetByClubIdAsync(clubId.Value);
            }
            else
            {
                participantes = await _participanteRepository.GetAllAsync();
            }

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
            // SaaS Enforcement: Verificar límites del plan
            if (participanteDto.ClubId.HasValue)
            {
                var clubId = participanteDto.ClubId.Value;
                var club = await _clubRepository.GetByIdAsync(clubId);
                
                if (club?.PlanSaaS != null)
                {
                    var count = await _participanteRepository.CountByClubIdAsync(clubId);
                    if (club.PlanSaaS.MaxAtletas != -1 && count >= club.PlanSaaS.MaxAtletas)
                    {
                        throw new BadRequestException($"Has alcanzado el límite de {club.PlanSaaS.MaxAtletas} atletas permitidos en tu plan {club.PlanSaaS.Nombre}.");
                    }
                }
            }

            var participante = _mapper.Map<Entidades.Entidades.Participante>(participanteDto);
            var result = await _participanteRepository.CreateAsync(participante);
            
            // Auditoria
            await _auditService.RegistrarAccionAsync("CREATE_ATHLETE", 
                $"Atleta creado: {result.Nombre} {result.Apellido} (DNI: {result.Dni})", null, "Atletas");

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

            // Auditoria
            await _auditService.RegistrarAccionAsync("UPDATE_ATHLETE", 
                $"Atleta actualizado: {result.Nombre} {result.Apellido} (ID: {id})", null, "Atletas");

            return _mapper.Map<ParticipanteDto>(result);
        }

        public async Task<bool> DeleteParticipanteAsync(int id)
        {
            var existing = await _participanteRepository.GetByIdAsync(id);
            if (existing == null) throw new NotFoundException($"Participante con ID {id} no encontrado");
            
            var res = await _participanteRepository.DeleteAsync(id);

            // Auditoria
            if (res) {
                await _auditService.RegistrarAccionAsync("DELETE_ATHLETE", 
                    $"Atleta eliminado: {existing.Nombre} {existing.Apellido} (DNI: {existing.Dni})", null, "Atletas");
            }

            return res;
        }
    }
}
