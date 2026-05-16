using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using System.Linq;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin,soporte_tecnico")]
    public class DiagnosticController : ControllerBase
    {
        private readonly SportTrackDbContext _context;

        public DiagnosticController(SportTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet("check-eventos")]
        public async Task<IActionResult> CheckEventos()
        {
            var eventos = await _context.Eventos
                .Include(e => e.EventoPruebas)
                    .ThenInclude(ep => ep.Prueba)
                .Select(e => new {
                    e.Id,
                    e.Nombre,
                    e.Fecha,
                    e.Estado,
                    PruebasCount = e.EventoPruebas.Count,
                    Pruebas = e.EventoPruebas.Select(ep => new {
                        ep.Id,
                        NombrePrueba = ep.Prueba.Nombre,
                        FasesCount = _context.Fases.Count(f => f.Etapa.EventoPruebaId == ep.Id)
                    })
                })
                .ToListAsync();

            return Ok(eventos);
        }

        [HttpGet("search/{query}")]
        public async Task<IActionResult> Search(string query)
        {
            var eventos = await _context.Eventos
                .Where(e => EF.Functions.ILike(e.Nombre, $"%{query}%"))
                .ToListAsync();

            var pruebas = await _context.Pruebas
                .Where(p => EF.Functions.ILike(p.Nombre, $"%{query}%"))
                .ToListAsync();

            var fases = await _context.Fases
                .Where(f => EF.Functions.ILike(f.NombreFase, $"%{query}%"))
                .ToListAsync();

            return Ok(new {
                Eventos = eventos,
                Pruebas = pruebas,
                Fases = fases
            });
        }
    }
}
