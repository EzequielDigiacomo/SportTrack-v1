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

        public async Task<IEnumerable<Entidades.Entidades.Evento>> GetAllAsync(int? clubId = null, string? rol = null)
        {
            var query = _context.Eventos.AsQueryable();
            
            if (rol != "SuperAdmin" && clubId.HasValue)
            {
                var clubActual = await _context.Clubes.FirstOrDefaultAsync(c => c.Id == clubId.Value);
                if (clubActual != null)
                {
                    int federationId = clubActual.ParentClubId ?? clubActual.Id;

                    // Si el rol es uno de administración de competencias, ve toda la federación
                    var rolesAdministrativos = new[] { "Admin", "Largador", "Cronometrista", "JuezControl", "Control" };
                    if (rolesAdministrativos.Any(r => r.Equals(rol?.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        var clubIds = await _context.Clubes
                            .Where(c => c.Id == federationId || c.ParentClubId == federationId)
                            .Select(c => c.Id)
                            .ToListAsync();
                        
                        query = query.Where(e => e.ClubId.HasValue && clubIds.Contains(e.ClubId.Value));
                    }
                    else
                    {
                        query = query.Where(e => e.ClubId == clubId.Value || e.ClubId == federationId);
                    }
                }
                else
                {
                    // If club is invalid/0, don't return any events
                    query = query.Where(e => e.Id == -1);
                }
            }

            return await query
                .Include(e => e.Club)
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

        public async Task<IEnumerable<Entidades.Entidades.Evento>> GetProximosAsync(int? clubId = null, string? rol = null)
        {
            var query = _context.Eventos
                .Where(e => e.Fecha >= DateTime.UtcNow.Date);

            if (rol != "SuperAdmin" && clubId.HasValue)
            {
                var clubActual = await _context.Clubes.FirstOrDefaultAsync(c => c.Id == clubId.Value);
                if (clubActual != null)
                {
                    int federationId = clubActual.ParentClubId ?? clubActual.Id;

                    var rolesAdministrativos = new[] { "Admin", "Largador", "Cronometrista", "JuezControl", "Control" };
                    if (rolesAdministrativos.Any(r => r.Equals(rol?.Trim(), StringComparison.OrdinalIgnoreCase)))
                    {
                        var clubIds = await _context.Clubes
                            .Where(c => c.Id == federationId || c.ParentClubId == federationId)
                            .Select(c => c.Id)
                            .ToListAsync();
                        
                        query = query.Where(e => e.ClubId.HasValue && clubIds.Contains(e.ClubId.Value));
                    }
                    else // Club normal
                    {
                        query = query.Where(e => e.ClubId == clubId.Value || e.ClubId == federationId);
                    }
                }
                else
                {
                    // Invalid club, return empty
                    query = query.Where(e => e.Id == -1);
                }
            }

            return await query
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
