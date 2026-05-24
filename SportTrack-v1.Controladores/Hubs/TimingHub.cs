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
        public async Task RequestStartRace(int faseId, DateTime startTime)
        {
            // Ejecutamos la lógica de inicio en el servicio (DB update, etc)
            // Pasamos la hora de inicio capturada por el largador
            var fase = await _faseService.IniciarFaseAsync(faseId, startTime);
        }

        public async Task RequestResetRace(int faseId)
        {
            await _faseService.ReiniciarFaseAsync(faseId);
            // El servicio emite "RaceReset"
        }

        // Notificaciones y Sincronización
        public DateTime GetServerTime()
        {
            return DateTime.UtcNow;
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
            await _faseService.UpdateResultadoStatusAsync(int.Parse(resultadoId), status);
            // El servicio ya emite "GlobalResultStatusUpdated"
        }

        public async Task RequestPaymentStatusChange(string clubNombre, string clubId)
        {
            await Clients.All.SendAsync("paymentStatusChangeRequested", new { clubNombre, clubId, motive = "solicitar cambio de estado de pago de este club" });
        }
    }
}
