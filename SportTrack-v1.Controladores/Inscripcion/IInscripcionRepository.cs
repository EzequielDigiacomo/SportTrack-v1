using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Inscripcion
{
    public interface IInscripcionRepository
    {
        Task<Entidades.Entidades.Inscripcion?> GetByIdAsync(int id);
        Task<IEnumerable<Entidades.Entidades.Inscripcion>> GetAllAsync();
        Task<Entidades.Entidades.Inscripcion> CreateAsync(Entidades.Entidades.Inscripcion inscripcion);
        Task<Entidades.Entidades.Inscripcion> UpdateAsync(Entidades.Entidades.Inscripcion inscripcion);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        
        // Método solicitado para contabilizar inscripciones
        Task<int> CountByEventoPruebaIdAsync(int eventoPruebaId);
        
        Task<IEnumerable<Entidades.Entidades.Inscripcion>> GetByEventoPruebaIdAsync(int eventoPruebaId);
        Task<IEnumerable<Entidades.Entidades.Inscripcion>> GetByEventoAndClubAsync(int eventoId, int clubId);
    }
}
