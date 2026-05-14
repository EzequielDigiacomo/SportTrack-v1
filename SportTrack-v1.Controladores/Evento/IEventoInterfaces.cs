using SportTrack_v1.Controladores.Evento.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Evento
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Entidades.Entidades.Evento>> GetAllAsync(int? clubId = null, string? rol = null);
        Task<Entidades.Entidades.Evento?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Evento> CreateAsync(Entidades.Entidades.Evento evento);
        Task<Entidades.Entidades.Evento> UpdateAsync(Entidades.Entidades.Evento evento);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Entidades.Entidades.Evento>> GetProximosAsync(int? clubId = null, string? rol = null);
        Task<IEnumerable<Entidades.Entidades.EventoPrueba>> GetPruebasByEventoIdAsync(int eventoId);
        Task<Entidades.Entidades.EventoPrueba?> GetEventoPruebaByIdAsync(int id);
        Task<Entidades.Entidades.EventoPrueba> AssignPruebaAsync(Entidades.Entidades.EventoPrueba eventoPrueba);
        Task<Entidades.Entidades.EventoPrueba> UpdateEventoPruebaAsync(Entidades.Entidades.EventoPrueba eventoPrueba);
        Task<bool> UnassignPruebaAsync(int id);
        Task<Entidades.Entidades.Prueba?> GetPruebaAsync(int categoriaId, int boteId, int distanciaId, int sexoId);
        Task<Entidades.Entidades.Prueba> CreatePruebaAsync(Entidades.Entidades.Prueba prueba);
    }

    public interface IEventoService
    {
        Task<IEnumerable<EventoDto>> GetAllEventosAsync(int? clubId = null, string? rol = null);
        Task<EventoDto> GetEventoByIdAsync(int id);
        Task<EventoDto> CreateEventoAsync(EventoCreateDto eventoDto);
        Task<EventoDto> UpdateEventoAsync(int id, EventoUpdateDto eventoDto, int? clubId = null);
        Task<bool> DeleteEventoAsync(int id, int? clubId = null);
        Task<IEnumerable<EventoDto>> GetProximosEventosAsync(int? clubId = null, string? rol = null);
        Task<IEnumerable<EventoPruebaDto>> GetPruebasByEventoAsync(int eventoId);
        Task<EventoPruebaDto> AssignPruebaToEventoAsync(int eventoId, EventoPruebaCreateDto assignDto);
        Task<EventoPruebaDto> UpdateEventoPruebaAsync(int eventoPruebaId, EventoPruebaCreateDto updateDto);
        Task<bool> DeleteEventoPruebaAsync(int eventoPruebaId);
    }
}
