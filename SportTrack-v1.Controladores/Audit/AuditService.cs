using Microsoft.AspNetCore.Http;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using System;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Audit
{
    public class AuditService : IAuditService
    {
        private readonly SportTrackDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(SportTrackDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RegistrarAccionAsync(string accion, string detalle, string usuario = "System", string modulo = "General")
        {
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var currentUserName = user?.Identity?.Name 
                                    ?? user?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value 
                                    ?? user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                                    ?? user?.FindFirst("nameid")?.Value 
                                    ?? "System/Anonymous";

                var audit = new Auditoria
                {
                    Accion = accion,
                    Detalle = detalle,
                    Usuario = usuario ?? currentUserName,
                    Modulo = modulo,
                    Fecha = DateTime.UtcNow,
                    IP = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0"
                };

                _context.Auditoria.Add(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Silenciamos errores de auditoria para no bloquear el flujo principal, 
                // pero podriamos loguearlos en un file system si fuera critico.
                Console.WriteLine($"Error en Auditoria: {ex.Message}");
            }
        }
    }
}
