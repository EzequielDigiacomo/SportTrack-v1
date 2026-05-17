using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Participante
{
    public class ParticipanteRepository : IParticipanteRepository
    {
        private readonly SportTrackDbContext _context;

        public ParticipanteRepository(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Entidades.Entidades.Participante>> GetAllAsync()
        {
            var list = await _context.Participantes
                .Include(p => p.Sexo)
                .Include(p => p.Categoria)
                .Include(p => p.Club)
                .AsNoTracking()
                .ToListAsync();

            // Corrección al vuelo para atletas afectados por el hueco de edad (36-39)
            foreach (var p in list)
            {
                if (p.CategoriaId == 11 && p.Edad >= 36 && p.Edad <= 39)
                {
                    p.Categoria = new Entidades.Entidades.Categoria { Id = 7, Nombre = "Senior" };
                }
            }
            return list;
        }

        public async Task<Entidades.Entidades.Participante?> GetByIdAsync(int id)
        {
            var p = await _context.Participantes
                .Include(p => p.Sexo)
                .Include(p => p.Categoria)
                .Include(p => p.Club)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p != null && p.CategoriaId == 11 && p.Edad >= 36 && p.Edad <= 39)
            {
                p.Categoria = new Entidades.Entidades.Categoria { Id = 7, Nombre = "Senior" };
            }
            return p;
        }

        public async Task<IEnumerable<Entidades.Entidades.Participante>> GetByClubIdAsync(int clubId)
        {
            var list = await _context.Participantes
                .Include(p => p.Sexo)
                .Include(p => p.Categoria)
                .Include(p => p.Club)
                .Where(p => p.ClubId == clubId)
                .AsNoTracking()
                .ToListAsync();

            // Corrección al vuelo para la grilla
            foreach (var p in list)
            {
                if (p.CategoriaId == 11 && p.Edad >= 36 && p.Edad <= 39)
                {
                    p.Categoria = new Entidades.Entidades.Categoria { Id = 7, Nombre = "Senior" };
                }
            }
            return list;
        }

        public async Task<IEnumerable<Entidades.Entidades.Participante>> GetByFederationIdAsync(int federationId)
        {
            // Obtener IDs de todos los clubes que tienen a esta federación como padre + el ID de la propia federación
            var clubIds = await _context.Clubes
                .Where(c => c.Id == federationId || c.ParentClubId == federationId)
                .Select(c => c.Id)
                .ToListAsync();

            var list = await _context.Participantes
                .Include(p => p.Sexo)
                .Include(p => p.Categoria)
                .Include(p => p.Club)
                .Where(p => !p.ClubId.HasValue || clubIds.Contains(p.ClubId.Value))
                .AsNoTracking()
                .ToListAsync();

            // Corrección al vuelo para la grilla
            foreach (var p in list)
            {
                if (p.CategoriaId == 11 && p.Edad >= 36 && p.Edad <= 39)
                {
                    p.Categoria = new Entidades.Entidades.Categoria { Id = 7, Nombre = "Senior" };
                }
            }
            return list;
        }

        public async Task<Entidades.Entidades.Participante> CreateAsync(Entidades.Entidades.Participante participante)
        {
            var edad = DateTime.UtcNow.Year - participante.FechaNacimiento.Year;
            var categoria = await _context.Categorias
                .Where(c => c.Nombre != "Control")
                .FirstOrDefaultAsync(c => 
                    (c.EdadMin == null || edad >= c.EdadMin) && 
                    (c.EdadMax == null || edad <= c.EdadMax));
            
            if (categoria != null)
            {
                participante.CategoriaId = categoria.Id;
            }

            _context.Participantes.Add(participante);
            await _context.SaveChangesAsync();
            return participante;
        }

        public async Task<Entidades.Entidades.Participante> UpdateAsync(Entidades.Entidades.Participante participante)
        {
            var edad = DateTime.UtcNow.Year - participante.FechaNacimiento.Year;
            var categoria = await _context.Categorias
                .Where(c => c.Nombre != "Control")
                .FirstOrDefaultAsync(c => 
                    (c.EdadMin == null || edad >= c.EdadMin) && 
                    (c.EdadMax == null || edad <= c.EdadMax));
            
            if (categoria != null)
            {
                participante.CategoriaId = categoria.Id;
            }

            _context.Entry(participante).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return participante;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var participante = await _context.Participantes.FindAsync(id);
            if (participante == null) return false;
            _context.Participantes.Remove(participante);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Participantes.AnyAsync(p => p.Id == id);
        }

        public async Task<int> CountByClubIdAsync(int clubId)
        {
            return await _context.Participantes.CountAsync(p => p.ClubId == clubId);
        }
    }
}
