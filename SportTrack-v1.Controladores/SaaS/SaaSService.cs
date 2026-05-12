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

            var clubes = await _context.Clubes
                .Include(c => c.PlanSaaS)
                .Include(c => c.Participantes)
                .ToListAsync();

            // Calculamos torneos activos por club
            // Un torneo activo es un Evento en estado Programada o EnCurso
            var eventosPorClub = await _context.Eventos
                .Where(e => (e.Estado == Entidades.Enums.EstadoEventoEnum.Programada || e.Estado == Entidades.Enums.EstadoEventoEnum.EnCurso) && e.ClubId.HasValue)
                .GroupBy(e => e.ClubId.Value)
                .Select(g => new { ClubId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.ClubId, g => g.Count);

            return clubes.Select(c => 
            {
                var planActivo = c.PlanSaaS ?? planBasico;
                var maxAtletas = planActivo?.MaxAtletas ?? 500;
                var maxTorneos = planActivo?.MaxTorneosActivos ?? 1;

                var atletasRegistrados = c.Participantes.Count;
                var torneosActivos = eventosPorClub.ContainsKey(c.Id) ? eventosPorClub[c.Id] : 0;

                var alDia = true;
                if (maxAtletas != -1 && atletasRegistrados > maxAtletas) alDia = false;
                if (maxTorneos != -1 && torneosActivos > maxTorneos) alDia = false;

                return new ClubSaaSStatusDto
                {
                    ClubId = c.Id,
                    ClubNombre = c.Nombre,
                    PlanSaaSId = planActivo?.Id,
                    PlanNombre = planActivo?.Nombre ?? "Desconocido",
                    MaxAtletas = maxAtletas,
                    AtletasRegistrados = atletasRegistrados,
                    MaxTorneos = maxTorneos,
                    TorneosActivos = torneosActivos,
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
