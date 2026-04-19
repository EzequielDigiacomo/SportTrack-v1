using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Hubs
{
    public class ResultadosHub : Hub
    {
        // Los clientes llaman a este método para suscribirse a una prueba específica
        public async Task JoinPruebaGroup(int pruebaId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, pruebaId.ToString());
        }

        // Los clientes llaman a este método para desuscribirse de una prueba específica
        public async Task LeavePruebaGroup(int pruebaId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, pruebaId.ToString());
        }
    }
}
