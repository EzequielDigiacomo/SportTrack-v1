using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Audit
{
    public interface IAuditService
    {
        Task RegistrarAccionAsync(string accion, string detalle, string? usuario = null, string modulo = "General");
        Task RegistrarErrorAsync(Exception ex, string modulo = "System");
    }
}
