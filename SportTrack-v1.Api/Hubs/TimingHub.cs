using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Hubs
{
    public class TimingHub : Hub
    {
        // Une al juez a un grupo específico para la prueba/fase actual
        public async Task JoinRaceGroup(string faseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"race_{faseId}");
        }

        // Notifica que la carrera ha comenzado
        public async Task StartRace(string faseId, DateTime startTime)
        {
            await Clients.Group($"race_{faseId}").SendAsync("RaceStarted", faseId, startTime);
        }

        // Notifica que un atleta cruzó la meta
        public async Task RecordLap(string faseId, int inscripcionId, string time)
        {
            await Clients.Group($"race_{faseId}").SendAsync("LapRecorded", inscripcionId, time);
        }

        // Notifica que la carrera terminó
        public async Task FinishRace(string faseId)
        {
            await Clients.Group($"race_{faseId}").SendAsync("RaceFinished", faseId);
        }
    }
}
