using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.SaaS.Dtos;
using SportTrack_v1.Entidades.Entidades;

namespace SportTrack_v1.Controladores.SaaS
{
    public class SaaSService : ISaaSService
    {
        private readonly SportTrackDbContext _context;

        public SaaSService(SportTrackDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PlanSaaSDto>> GetPlanesAsync()
        {
            var planes = await _context.PlanesSaaS.ToListAsync();
            return planes.Select(p => new PlanSaaSDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                MaxAtletas = p.MaxAtletas,
                MaxTorneosActivos = p.MaxTorneosActivos,
                ResultadosTiempoReal = p.ResultadosTiempoReal,
                ExportacionExcel = p.ExportacionExcel,
                SoportePrioritario = p.SoportePrioritario
            });
        }

        public async Task<PlanSaaSDto> GetPlanByIdAsync(int id)
        {
            var p = await _context.PlanesSaaS.FindAsync(id);
            if (p == null) return null;

            return new PlanSaaSDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                MaxAtletas = p.MaxAtletas,
                MaxTorneosActivos = p.MaxTorneosActivos,
                ResultadosTiempoReal = p.ResultadosTiempoReal,
                ExportacionExcel = p.ExportacionExcel,
                SoportePrioritario = p.SoportePrioritario
            };
        }

        public async Task AsignarPlanAClubAsync(int clubId, int planId)
        {
            var club = await _context.Clubes.FindAsync(clubId);
            if (club != null)
            {
                club.PlanSaaSId = planId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ClubSaaSStatusDto>> GetClubesStatusAsync()
        {
            // Plan basico por defecto si no tiene plan (ID 1)
            var planBasico = await _context.PlanesSaaS.FirstOrDefaultAsync(p => p.Id == 1);

            var federaciones = await _context.Clubes
                .Where(c => c.ParentClubId == null) // Solo las federaciones "madre"
                .Include(c => c.PlanSaaS)
                .Include(c => c.Participantes)
                .Include(c => c.Usuarios)
                .Include(c => c.Afiliados)
                    .ThenInclude(a => a.Participantes)
                .Include(c => c.Afiliados)
                    .ThenInclude(a => a.Usuarios)
                .ToListAsync();

            // Buscamos todos los torneos activos para agruparlos por federación madre
            var eventosActivos = await _context.Eventos
                .Where(e => (e.Estado == Entidades.Enums.EstadoEventoEnum.Programada || e.Estado == Entidades.Enums.EstadoEventoEnum.EnCurso) && e.ClubId.HasValue)
                .Select(e => new { e.ClubId, e.Id, e.Nombre, e.Fecha, Estado = e.Estado.ToString() })
                .ToListAsync();

            return federaciones.Select(c => 
            {
                var planActivo = c.PlanSaaS ?? planBasico;
                var maxAtletas = planActivo?.MaxAtletas ?? 500;
                var maxTorneos = planActivo?.MaxTorneosActivos ?? 1;

                // Identificamos todos los IDs que pertenecen a esta federación (ella misma + sus afiliados)
                var idsPertenecientes = new HashSet<int> { c.Id };
                foreach (var af in c.Afiliados) idsPertenecientes.Add(af.Id);

                // Agregamos métricas de afiliados
                var atletasRegistrados = c.Participantes.Count + c.Afiliados.Sum(a => a.Participantes.Count);
                var usuariosCount = c.Usuarios.Count + c.Afiliados.Sum(a => a.Usuarios.Count);
                
                var torneosDetalle = eventosActivos
                    .Where(e => idsPertenecientes.Contains(e.ClubId.Value))
                    .Select(e => new TorneoSaaSDetailDto { Id = e.Id, Nombre = e.Nombre, Fecha = e.Fecha, Estado = e.Estado })
                    .ToList();
                
                var torneosActivosCount = torneosDetalle.Count;

                var alDia = true;
                if (maxAtletas != -1 && atletasRegistrados > maxAtletas) alDia = false;
                if (maxTorneos != -1 && torneosActivosCount > maxTorneos) alDia = false;

                return new ClubSaaSStatusDto
                {
                    ClubId = c.Id,
                    ClubNombre = c.Nombre,
                    PlanSaaSId = planActivo?.Id,
                    PlanNombre = planActivo?.Nombre ?? "Desconocido",
                    MaxAtletas = maxAtletas,
                    AtletasRegistrados = atletasRegistrados,
                    ClubesAfiliadosCount = c.Afiliados.Count,
                    UsuariosCount = usuariosCount,
                    MaxTorneos = maxTorneos,
                    TorneosActivosCount = torneosActivosCount,
                    TorneosActivos = torneosDetalle,
                    PlanAlDia = alDia,
                    Activo = c.Activo
                };
            });
        }

        public async Task ToggleClubActivoAsync(int clubId)
        {
            var club = await _context.Clubes.FindAsync(clubId);
            if (club != null)
            {
                club.Activo = !club.Activo;
                await _context.SaveChangesAsync();
            }
        }
    }
}
