using SportTrack_v1.Controladores.Inscripcion.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Inscripcion
{
    public interface IInscripcionService
    {
        Task<IEnumerable<InscripcionDto>> GetAllInscripcionesAsync();
        Task<InscripcionDto> GetInscripcionByIdAsync(int id);
        Task<InscripcionDto> CreateInscripcionAsync(InscripcionCreateDto inscripcionDto);
        Task<InscripcionDto> UpdateInscripcionAsync(int id, InscripcionUpdateDto inscripcionDto);
        Task<bool> DeleteInscripcionAsync(int id);
        Task<int> GetCountByEventoPruebaIdAsync(int eventoPruebaId);
        Task<IEnumerable<InscripcionDto>> GetInscripcionesByEventoPruebaIdAsync(int eventoPruebaId);
        Task<IEnumerable<InscripcionDto>> GetInscripcionesByEventoAndClubAsync(int eventoId, int clubId);
        Task<bool> ToggleEsCabezaDeSerieAsync(int id);
    }
}
