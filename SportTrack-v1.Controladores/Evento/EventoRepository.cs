using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Evento
{
    public class EventoRepository : IEventoRepository
    {
        private readonly SportTrackDbContext _context;

        public EventoRepository(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entidades.Entidades.Evento>> GetAllAsync(int? clubId = null)
        {
            var query = _context.Eventos.AsQueryable();
            
            if (clubId.HasValue)
            {
                query = query.Where(e => e.ClubId == clubId.Value);
            }

            return await query
                .AsNoTracking()
                .OrderByDescending(e => e.Fecha)
                .ToListAsync();
        }

        public async Task<Entidades.Entidades.Evento?> GetByIdAsync(int id)
        {
            return await _context.Eventos
                .Include(e => e.EventoPruebas)
                    .ThenInclude(ep => ep.Prueba)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Entidades.Entidades.Evento> CreateAsync(Entidades.Entidades.Evento evento)
        {
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<Entidades.Entidades.Evento> UpdateAsync(Entidades.Entidades.Evento evento)
        {
            _context.Entry(evento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return evento;
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null) return false;
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Eventos.AnyAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Entidades.Entidades.Evento>> GetProximosAsync()
        {
            return await _context.Eventos
                .Where(e => e.Fecha >= DateTime.UtcNow)
                .OrderBy(e => e.Fecha)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EventoPrueba>> GetPruebasByEventoIdAsync(int eventoId)
        {
            return await _context.EventoPruebas
                .Include(ep => ep.Prueba)
                    .ThenInclude(p => p.Categoria)
                .Include(ep => ep.Prueba)
                    .ThenInclude(p => p.Bote)
                .Include(ep => ep.Prueba)
                    .ThenInclude(p => p.Distancia)
                .Include(ep => ep.Prueba)
                    .ThenInclude(p => p.Sexo)
                .Include(ep => ep.Inscripciones)
                .Where(ep => ep.EventoId == eventoId)
                .OrderBy(ep => ep.FechaHora)
                .ToListAsync();
        }

        public async Task<EventoPrueba?> GetEventoPruebaByIdAsync(int id)
        {
            return await _context.EventoPruebas
                .Include(ep => ep.Evento)
                .FirstOrDefaultAsync(ep => ep.Id == id);
        }

        public async Task<EventoPrueba> AssignPruebaAsync(EventoPrueba eventoPrueba)
        {
            _context.EventoPruebas.Add(eventoPrueba);
            await _context.SaveChangesAsync();
            return eventoPrueba;
        }

        public async Task<EventoPrueba> UpdateEventoPruebaAsync(EventoPrueba eventoPrueba)
        {
            _context.Entry(eventoPrueba).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return eventoPrueba;
        }

        public async Task<bool> UnassignPruebaAsync(int id)
        {
            var entity = await _context.EventoPruebas.FindAsync(id);
            if (entity == null) return false;
            
            _context.EventoPruebas.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Entidades.Entidades.Prueba?> GetPruebaAsync(int categoriaId, int boteId, int distanciaId, int sexoId)
        {
            return await _context.Pruebas
                .FirstOrDefaultAsync(p => p.CategoriaId == categoriaId && 
                                        p.BoteId == boteId && 
                                        p.DistanciaId == distanciaId && 
                                        p.SexoId == sexoId);
        }

        public async Task<Entidades.Entidades.Prueba> CreatePruebaAsync(Entidades.Entidades.Prueba prueba)
        {
            _context.Pruebas.Add(prueba);
            await _context.SaveChangesAsync();
            return prueba;
        }
    }
}
