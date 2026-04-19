using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SportTrack_v1.Api.Hubs
{
    public class ResultsHub : Hub
    {
        public async Task JoinEventGroup(string eventId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, eventId);
        }

        public async Task LeaveEventGroup(string eventId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, eventId);
        }
    }
}
