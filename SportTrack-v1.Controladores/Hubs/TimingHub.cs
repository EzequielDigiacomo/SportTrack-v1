using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using SportTrack_v1.Controladores.Fase;

namespace SportTrack_v1.Controladores.Hubs
{
    public class RaceUserPresence
    {
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
    }

    public class TimingHub : Hub
    {
        private readonly IFaseService _faseService;
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<RaceUserPresence>> _activeRaceGroups = 
            new System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<RaceUserPresence>>();

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<RaceUserPresence>> _activeEventGroups = 
            new System.Collections.Concurrent.ConcurrentDictionary<string, System.Collections.Generic.List<RaceUserPresence>>();

        public TimingHub(IFaseService faseService)
        {
            _faseService = faseService;
        }

        public async Task JoinRaceGroup(string faseId, string userName, string role)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"race_{faseId}");

            var presence = new RaceUserPresence
            {
                ConnectionId = Context.ConnectionId,
                UserName = userName,
                Role = role
            };

            _activeRaceGroups.AddOrUpdate(faseId,
                new System.Collections.Generic.List<RaceUserPresence> { presence },
                (key, oldValue) =>
                {
                    lock (oldValue)
                    {
                        oldValue.RemoveAll(x => x.ConnectionId == Context.ConnectionId || (x.UserName == userName && x.Role == role));
                        oldValue.Add(presence);
                    }
                    return oldValue;
                });

            if (_activeRaceGroups.TryGetValue(faseId, out var currentList))
            {
                System.Collections.Generic.List<RaceUserPresence> copyList;
                lock (currentList)
                {
                    copyList = new System.Collections.Generic.List<RaceUserPresence>(currentList);
                }
                await Clients.Group($"race_{faseId}").SendAsync("RacePresenceUpdated", copyList);
            }
        }

        public async Task JoinEventGroup(string eventoId, string userName, string role)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"event_{eventoId}");

            var presence = new RaceUserPresence
            {
                ConnectionId = Context.ConnectionId,
                UserName = userName,
                Role = role
            };

            _activeEventGroups.AddOrUpdate(eventoId,
                new System.Collections.Generic.List<RaceUserPresence> { presence },
                (key, oldValue) =>
                {
                    lock (oldValue)
                    {
                        oldValue.RemoveAll(x => x.ConnectionId == Context.ConnectionId || (x.UserName == userName && x.Role == role));
                        oldValue.Add(presence);
                    }
                    return oldValue;
                });

            if (_activeEventGroups.TryGetValue(eventoId, out var currentList))
            {
                System.Collections.Generic.List<RaceUserPresence> copyList;
                lock (currentList)
                {
                    copyList = new System.Collections.Generic.List<RaceUserPresence>(currentList);
                }
                await Clients.Group($"event_{eventoId}").SendAsync("EventPresenceUpdated", copyList);
            }
        }

        public async Task LeaveRaceGroup(string faseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"race_{faseId}");

            if (_activeRaceGroups.TryGetValue(faseId, out var currentList))
            {
                lock (currentList)
                {
                    currentList.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
                }
                System.Collections.Generic.List<RaceUserPresence> copyList;
                lock (currentList)
                {
                    copyList = new System.Collections.Generic.List<RaceUserPresence>(currentList);
                }
                await Clients.Group($"race_{faseId}").SendAsync("RacePresenceUpdated", copyList);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            foreach (var entry in _activeRaceGroups)
            {
                var faseId = entry.Key;
                var currentList = entry.Value;
                bool removed = false;
                lock (currentList)
                {
                    int before = currentList.Count;
                    currentList.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
                    removed = currentList.Count < before;
                }
                if (removed)
                {
                    System.Collections.Generic.List<RaceUserPresence> copyList;
                    lock (currentList)
                    {
                        copyList = new System.Collections.Generic.List<RaceUserPresence>(currentList);
                    }
                    await Clients.Group($"race_{faseId}").SendAsync("RacePresenceUpdated", copyList);
                }
            }

            foreach (var entry in _activeEventGroups)
            {
                var eventoId = entry.Key;
                var currentList = entry.Value;
                bool removed = false;
                lock (currentList)
                {
                    int before = currentList.Count;
                    currentList.RemoveAll(x => x.ConnectionId == Context.ConnectionId);
                    removed = currentList.Count < before;
                }
                if (removed)
                {
                    System.Collections.Generic.List<RaceUserPresence> copyList;
                    lock (currentList)
                    {
                        copyList = new System.Collections.Generic.List<RaceUserPresence>(currentList);
                    }
                    await Clients.Group($"event_{eventoId}").SendAsync("EventPresenceUpdated", copyList);
                }
            }

            await base.OnDisconnectedAsync(exception);
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
