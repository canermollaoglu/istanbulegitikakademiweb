using Microsoft.AspNetCore.SignalR;

namespace NitelikliBilisim.App.Hubs
{
    public class MessageHub : Hub
    {
        public void BroadcastMessage(string name, string message)
        {
            Clients.All.SendAsync("broadcastMessage", name, message);
        }
    }
}
