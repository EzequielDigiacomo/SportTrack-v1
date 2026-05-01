using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using SportTrack_v1.Controladores.Fase;

namespace SportTrack_v1.Controladores.Hubs
{
    public class TimingHub : Hub
    {
        private readonly IFaseService _faseService;

        public TimingHub(IFaseService faseService)
        {
            _faseService = faseService;
        }

        public async Task JoinRaceGroup(string faseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"race_{faseId}");
        }

        public async Task LeaveRaceGroup(string faseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"race_{faseId}");
        }

        // Acciones críticas vía WebSocket para mínima latencia
        public async Task RequestStartRace(int faseId)
        {
            // Ejecutamos la lógica de inicio en el servicio (DB update, etc)
            var fase = await _faseService.IniciarFaseAsync(faseId);
            // El servicio ya emite "RaceStarted" con el serverTime exacto
        }

        public async Task RequestResetRace(int faseId)
        {
            await _faseService.ReiniciarFaseAsync(faseId);
            // El servicio emite "RaceReset"
        }

        // Notificaciones y Sincronización
        public async Task GetServerTime()
        {
            await Clients.Caller.SendAsync("ReceiveServerTime", DateTime.UtcNow);
        }

        public async Task RecordLap(int faseId, int resultadoId, string time)
        {
            await Clients.Group($"race_{faseId}").SendAsync("LapRecorded", resultadoId, time);
        }

        public async Task FinishRace(int faseId)
        {
            await Clients.Group($"race_{faseId}").SendAsync("RaceFinished", faseId);
        }

        public async Task SendTime(string faseId, string resultadoId, string timeStr, long ms)
        {
            await Clients.Group($"race_{faseId}").SendAsync("TimeReceived", resultadoId, timeStr, ms);
        }

        public async Task UpdateResultStatus(string faseId, string resultadoId, string status)
        {
            await Clients.Group($"race_{faseId}").SendAsync("ResultStatusUpdated", resultadoId, status);
        }
    }
}
