using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Resultado
{
    public interface IResultadoRepository
    {
        Task<IEnumerable<Entidades.Entidades.Resultado>> GetByFaseIdAsync(int faseId);
        Task<Entidades.Entidades.Resultado?> GetByIdAsync(int id);
        Task<Entidades.Entidades.Resultado> UpdateAsync(Entidades.Entidades.Resultado resultado);
        Task<IEnumerable<Entidades.Entidades.Resultado>> UpdateManyAsync(IEnumerable<Entidades.Entidades.Resultado> resultados);
    }

    public class ResultadoRepository : IResultadoRepository
    {
        private readonly SportTrackDbContext _context;

        public ResultadoRepository(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entidades.Entidades.Resultado>> GetByFaseIdAsync(int faseId)
        {
            return await _context.Resultados
                .Include(r => r.Inscripcion)
                    .ThenInclude(i => i.Participante)
                        .ThenInclude(p => p.Club)
                .Where(r => r.FaseId == faseId)
                .OrderBy(r => r.Posicion ?? int.MaxValue)
                .ToListAsync();
        }

        public async Task<Entidades.Entidades.Resultado?> GetByIdAsync(int id)
        {
            return await _context.Resultados
                .Include(r => r.Fase)
                    .ThenInclude(f => f.Etapa)
                .Include(r => r.Inscripcion)
                    .ThenInclude(i => i.Participante)
                        .ThenInclude(p => p.Club)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Entidades.Entidades.Resultado> UpdateAsync(Entidades.Entidades.Resultado resultado)
        {
            _context.Entry(resultado).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return resultado;
        }

        public async Task<IEnumerable<Entidades.Entidades.Resultado>> UpdateManyAsync(IEnumerable<Entidades.Entidades.Resultado> resultados)
        {
            foreach (var res in resultados)
            {
                _context.Entry(res).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return resultados;
        }
    }
}
