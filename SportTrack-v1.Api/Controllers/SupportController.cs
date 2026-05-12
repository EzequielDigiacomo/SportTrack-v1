using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.Audit;
using System.Threading.Tasks;
using System.Linq;

namespace SportTrack_v1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Protegido inicialmente, luego filtraremos por rol o nombre
    public class SupportController : ControllerBase
    {
        private readonly SportTrackDbContext _context;

        public SupportController(SportTrackDbContext context)
        {
            _context = context;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] string modulo = null, [FromQuery] int limit = 100)
        {
            // Solo permitir a usuarios específicos o con el rol SuperAdmin
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userName = User.Identity?.Name;

            if (userRole != "SuperAdmin" && userName != "soporte_tecnico" && userName != "admin")
            {
                return Forbid("No tienes permisos para acceder a los registros de soporte.");
            }

            var query = _context.Auditoria.AsQueryable();

            if (!string.IsNullOrEmpty(modulo))
            {
                query = query.Where(a => a.Modulo == modulo);
            }

            var logs = await query
                .OrderByDescending(a => a.Fecha)
                .Take(limit)
                .ToListAsync();

            return Ok(logs);
        }

        [HttpPost("frontend-error")]
        [AllowAnonymous] // Permitir reportes incluso si no hay sesión (ej: falla el login)
        public async Task<IActionResult> PostFrontendError([FromBody] FrontendErrorDto errorDto)
        {
            var detail = new
            {
                Error = errorDto.Message,
                Url = errorDto.Url,
                Stack = errorDto.Stack,
                Browser = errorDto.BrowserInfo,
                User = User.Identity?.Name ?? "Anónimo"
            };

            await _context.Auditoria.AddAsync(new SportTrack_v1.Entidades.Entidades.Auditoria
            {
                Accion = "FRONTEND_CRASH",
                Modulo = "ReactApp",
                Detalle = System.Text.Json.JsonSerializer.Serialize(detail),
                Usuario = User.Identity?.Name ?? "Anónimo",
                Fecha = DateTime.UtcNow,
                IP = Request.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0"
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class FrontendErrorDto
        {
            public string Message { get; set; }
            public string Url { get; set; }
            public string Stack { get; set; }
            public string BrowserInfo { get; set; }
        }

        [HttpDelete("logs/clear")]
        public async Task<IActionResult> ClearLogs()
        {
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            if (userRole != "SuperAdmin") return Forbid();

            // Solo borramos los de tipo ERROR para no perder auditoría legal
            var logsToRemove = _context.Auditoria.Where(a => a.Accion == "ERROR_FATAL");
            _context.Auditoria.RemoveRange(logsToRemove);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logs de error eliminados." });
        }
    }
}
