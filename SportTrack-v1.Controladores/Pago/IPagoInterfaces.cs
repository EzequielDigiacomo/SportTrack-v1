using SportTrack_v1.Controladores.Pago.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportTrack_v1.Controladores.Pago
{
    public interface IPagoService
    {
        Task<IEnumerable<PagoDto>> GetHistorialPagosAsync(int? fedId, string? role);
        Task<PagoDto> RegistrarPagoAsync(RegistrarPagoDto dto, string registradoPor);
        Task<bool> ToggleClubPagoStatusAsync(int clubId, bool alDia);
        Task<bool> ToggleAtletaPagoStatusAsync(int participanteId, bool alDia);
        Task<bool> ToggleInscripcionPagoStatusAsync(int inscripcionId, bool pagado);
        Task<bool> SetSolicitudPagoPendienteAsync(int clubId, bool pendiente);
    }
}
