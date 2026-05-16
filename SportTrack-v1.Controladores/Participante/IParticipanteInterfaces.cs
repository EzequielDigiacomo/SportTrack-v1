using SportTrack_v1.Controladores.Participante.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Participante
{
    public interface IParticipanteRepository
    {
        Task<IEnumerable<Entidades.Entidades.Participante>> GetAllAsync();
        Task<Entidades.Entidades.Participante?> GetByIdAsync(int id);
        Task<IEnumerable<Entidades.Entidades.Participante>> GetByClubIdAsync(int clubId);
        Task<IEnumerable<Entidades.Entidades.Participante>> GetByFederationIdAsync(int federationId);
        Task<Entidades.Entidades.Participante> CreateAsync(Entidades.Entidades.Participante participante);
        Task<Entidades.Entidades.Participante> UpdateAsync(Entidades.Entidades.Participante participante);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<int> CountByClubIdAsync(int clubId);
    }

    public interface IParticipanteService
    {
        Task<IEnumerable<ParticipanteDto>> GetAllParticipantesAsync(int? clubId = null, string? rol = null);
        Task<ParticipanteDto> GetParticipanteByIdAsync(int id);
        Task<IEnumerable<ParticipanteDto>> GetParticipantesByClubAsync(int clubId);
        Task<ParticipanteDto> CreateParticipanteAsync(ParticipanteCreateDto participanteDto);
        Task<ParticipanteDto> UpdateParticipanteAsync(int id, ParticipanteCreateDto participanteDto);
        Task<bool> DeleteParticipanteAsync(int id);
    }
}
