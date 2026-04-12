using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Resultado
{
    public class ResultadoRepository : IResultadoRepository
    {
        private readonly SportTrackDbContext _context;

        public ResultadoRepository(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entidades.Entidades.Resultado>> GetAllAsync()
        {
            return await _context.Resultados
                .Include(r => r.Inscripcion)
                    .ThenInclude(i => i.Participante)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Entidades.Entidades.Resultado?> GetByIdAsync(int id)
        {
            return await _context.Resultados
                .Include(r => r.Inscripcion)
                    .ThenInclude(i => i.Participante)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Entidades.Entidades.Resultado?> GetByInscripcionIdAsync(int inscripcionId)
        {
            return await _context.Resultados
                .FirstOrDefaultAsync(r => r.InscripcionId == inscripcionId);
        }

        public async Task<Entidades.Entidades.Resultado> CreateAsync(Entidades.Entidades.Resultado resultado)
        {
            _context.Resultados.Add(resultado);
            await _context.SaveChangesAsync();
            return resultado;
        }

        public async Task<Entidades.Entidades.Resultado> UpdateAsync(Entidades.Entidades.Resultado resultado)
        {
            _context.Entry(resultado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return resultado;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resultado = await _context.Resultados.FindAsync(id);
            if (resultado == null) return false;
            _context.Resultados.Remove(resultado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Entidades.Entidades.Resultado>> GetByEventoPruebaIdAsync(int eventoPruebaId)
        {
            return await _context.Resultados
                .Include(r => r.Inscripcion)
                    .ThenInclude(i => i.Participante)
                .Include(r => r.Inscripcion)
                    .ThenInclude(i => i.Participante)
                        .ThenInclude(p => p.Club)
                .Where(r => r.Inscripcion.EventoPruebaId == eventoPruebaId)
                .OrderBy(r => r.Posicion ?? int.MaxValue)
                .ThenBy(r => r.TiempoOficial)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
