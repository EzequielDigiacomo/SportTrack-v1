using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Hubs
{
    public class ResultsHub : Hub
    {
        public async Task JoinRaceGroup(string faseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, faseId);
        }

        public async Task LeaveRaceGroup(string faseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, faseId);
        }

        public async Task RequestStartRace(int faseId)
        {
            // Transmite a todos los unidos a esta fase que la carrera inició
            await Clients.Group(faseId.ToString()).SendAsync("RaceStarted", faseId, System.DateTime.UtcNow);
        }

        public async Task RequestResetRace(int faseId)
        {
            await Clients.Group(faseId.ToString()).SendAsync("RaceReset", faseId);
        }

        public async Task SendTime(string faseId, string resultadoId, string timeStr, long ms)
        {
            await Clients.Group(faseId).SendAsync("RaceFinished", resultadoId, timeStr, ms);
        }

        public async Task GetServerTime()
        {
            await Clients.Caller.SendAsync("ReceiveServerTime", System.DateTime.UtcNow);
        }
    }
}
