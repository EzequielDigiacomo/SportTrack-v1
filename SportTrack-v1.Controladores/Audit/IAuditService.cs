using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Audit
{
    public interface IAuditService
    {
        Task RegistrarAccionAsync(string accion, string detalle, string usuario = "System", string modulo = "General");
    }
}
