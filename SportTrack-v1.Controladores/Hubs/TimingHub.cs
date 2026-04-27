using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace SportTrack_v1.Controladores.Hubs
{
    public class TimingHub : Hub
    {
        public async Task JoinRaceGroup(string faseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"race_{faseId}");
        }

        public async Task LeaveRaceGroup(string faseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"race_{faseId}");
        }

        // Métodos para que el servidor notifique a los clientes
        public async Task StartRace(int faseId, DateTime serverTime)
        {
            await Clients.Group($"race_{faseId}").SendAsync("RaceStarted", faseId, serverTime);
        }

        public async Task RecordLap(int faseId, int resultadoId, string time)
        {
            await Clients.Group($"race_{faseId}").SendAsync("LapRecorded", resultadoId, time);
        }

        public async Task FinishRace(int faseId)
        {
            await Clients.Group($"race_{faseId}").SendAsync("RaceFinished", faseId);
        }
    }
}
