using Microsoft.AspNetCore.SignalR;
using SportTrack_v1.Api.Hubs;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Services
{
    public interface INotificadorResultados
    {
        Task NotificarCambioResultado(int eventoPruebaId, object resultado);
        Task NotificarCambioEstructura(int eventoPruebaId);
    }

    public class NotificadorResultados : INotificadorResultados
    {
        private readonly IHubContext<ResultsHub> _hubContext;

        public NotificadorResultados(IHubContext<ResultsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificarCambioResultado(int eventoPruebaId, object resultado)
        {
            // Enviamos el mensaje al grupo específico del evento/prueba
            await _hubContext.Clients.Group(eventoPruebaId.ToString())
                .SendAsync("RecibirResultado", resultado);
            
            // También podemos enviar a un canal general si fuera necesario
            await _hubContext.Clients.All.SendAsync("GlobalUpdate", resultado);
        }

        public async Task NotificarCambioEstructura(int eventoPruebaId)
        {
            await _hubContext.Clients.Group(eventoPruebaId.ToString())
                .SendAsync("RecibirEstructura", new { timestamp = System.DateTime.Now });
        }
    }
}
