using Microsoft.AspNetCore.SignalR;

namespace SignalRUdemy.Hubs
{
    public class ExampleHub:Hub
    {
        public async Task BroadcastMessageToAllClient(string message)
        {
            await Clients.All.SendAsync("ReceiveMessageForAllClient", message);
        }
    }
}
