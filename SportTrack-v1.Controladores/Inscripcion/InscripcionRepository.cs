using Microsoft.EntityFrameworkCore;
using SportTrack_v1.Entidades.Entidades;
using SportTrack.AccessDatos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportTrack_v1.Controladores.Inscripcion;

namespace SportTrack_v1.Controladores.Inscripcion
{
    public class InscripcionRepository : IInscripcionRepository
    {
        private readonly SportTrackDbContext _context;

        public InscripcionRepository(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entidades.Entidades.Inscripcion>> GetAllAsync()
        {
            return await _context.Inscripciones
                .Include(i => i.Participante)
                    .ThenInclude(p => p.Club)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Evento)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Categoria)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Bote)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Distancia)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Sexo)
                .Include(i => i.Tripulantes)
                    .ThenInclude(t => t.Participante)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Entidades.Entidades.Inscripcion?> GetByIdAsync(int id)
        {
            return await _context.Inscripciones
                .Include(i => i.Participante)
                    .ThenInclude(p => p.Club)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Evento)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Categoria)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Bote)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Distancia)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Sexo)
                .Include(i => i.Tripulantes)
                    .ThenInclude(t => t.Participante)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<Entidades.Entidades.Inscripcion> CreateAsync(Entidades.Entidades.Inscripcion inscripcion)
        {
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();
            return inscripcion;
        }

        public async Task<Entidades.Entidades.Inscripcion> UpdateAsync(Entidades.Entidades.Inscripcion inscripcion)
        {
            _context.Entry(inscripcion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return inscripcion;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null)
                return false;

            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Inscripciones.AnyAsync(i => i.Id == id);
        }

        public async Task<int> CountByEventoPruebaIdAsync(int eventoPruebaId)
        {
            return await _context.Inscripciones
                .CountAsync(i => i.EventoPruebaId == eventoPruebaId);
        }

        public async Task<IEnumerable<Entidades.Entidades.Inscripcion>> GetByEventoPruebaIdAsync(int eventoPruebaId)
        {
            return await _context.Inscripciones
                .Include(i => i.Participante)
                    .ThenInclude(p => p.Club)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Evento)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Categoria)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Bote)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Distancia)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Sexo)
                .Include(i => i.Tripulantes)
                    .ThenInclude(t => t.Participante)
                .Where(i => i.EventoPruebaId == eventoPruebaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Entidades.Entidades.Inscripcion>> GetByEventoAndClubAsync(int eventoId, int clubId)
        {
            return await _context.Inscripciones
                .Include(i => i.Participante)
                    .ThenInclude(p => p.Club)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Evento)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Categoria)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Bote)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Distancia)
                .Include(i => i.EventoPrueba)
                    .ThenInclude(ep => ep.Prueba)
                        .ThenInclude(p => p.Sexo)
                .Include(i => i.Tripulantes)
                    .ThenInclude(t => t.Participante)
                .Where(i => i.EventoPrueba.EventoId == eventoId && 
                            (i.Participante.ClubId == clubId || i.Tripulantes.Any(t => t.Participante.ClubId == clubId)))
                .ToListAsync();
        }
    }
}
