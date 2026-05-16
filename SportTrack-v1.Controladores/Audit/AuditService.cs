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

        public async Task RegistrarAccionAsync(string accion, string detalle, string? usuario = null, string modulo = "General")
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
                    IP = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0",
                    UserAgent = _httpContextAccessor.HttpContext?.Request?.Headers["User-Agent"].ToString() ?? "N/A"
                };

                _context.Auditoria.Add(audit);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Auditoria: {ex.Message}");
            }
        }

        public async Task RegistrarErrorAsync(Exception ex, string modulo = "System")
        {
            var detalle = new
            {
                Error = ex.Message,
                StackTrace = ex.StackTrace,
                InnerError = ex.InnerException?.Message,
                Source = ex.Source
            };

            await RegistrarAccionAsync("ERROR_FATAL", System.Text.Json.JsonSerializer.Serialize(detalle), "System", modulo);
        }
    }
}
