using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Fase
{
    public interface IEtapaRepository
    {
        Task<IEnumerable<Etapa>> GetByEventoPruebaIdAsync(int eventoPruebaId);
        Task<Etapa> CreateAsync(Etapa etapa);
        Task<IEnumerable<Etapa>> CreateManyAsync(IEnumerable<Etapa> etapas);
        Task DeleteByEventoPruebaIdAsync(int eventoPruebaId);
        Task DeleteAsync(int id);
    }

    public class EtapaRepository : IEtapaRepository
    {
        private readonly SportTrackDbContext _context;

        public EtapaRepository(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Etapa>> GetByEventoPruebaIdAsync(int eventoPruebaId)
        {
            return await _context.Etapas
                .Include(e => e.Fases)
                    .ThenInclude(f => f.Resultados)
                        .ThenInclude(r => r.Inscripcion)
                .Where(e => e.EventoPruebaId == eventoPruebaId)
                .OrderBy(e => e.Orden)
                .ToListAsync();
        }

        public async Task<Etapa> CreateAsync(Etapa etapa)
        {
            _context.Etapas.Add(etapa);
            await _context.SaveChangesAsync();
            return etapa;
        }

        public async Task<IEnumerable<Etapa>> CreateManyAsync(IEnumerable<Etapa> etapas)
        {
            _context.Etapas.AddRange(etapas);
            await _context.SaveChangesAsync();
            return etapas;
        }

        public async Task DeleteByEventoPruebaIdAsync(int eventoPruebaId)
        {
            var etapas = await _context.Etapas.Where(e => e.EventoPruebaId == eventoPruebaId).ToListAsync();
            _context.Etapas.RemoveRange(etapas);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var etapa = await _context.Etapas.FindAsync(id);
            if (etapa != null)
            {
                _context.Etapas.Remove(etapa);
                await _context.SaveChangesAsync();
            }
        }
    }
}
